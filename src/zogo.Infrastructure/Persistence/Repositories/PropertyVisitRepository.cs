using Microsoft.EntityFrameworkCore;
using zogo.Application.DTOs.Visits;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Buyer;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class PropertyVisitRepository : IPropertyVisitRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyVisitRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PropertyVisit visit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(visit);
        await _context.PropertyVisits.AddAsync(visit, cancellationToken);
    }

    public async Task<PropertyVisit?> GetByIdAsync(Guid visitId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyVisits
            .Include(v => v.Property)
            .Include(v => v.Buyer)
            .Include(v => v.AssignedAgent)
            .FirstOrDefaultAsync(v => v.Id == visitId && v.DeletedAt == null, cancellationToken);
    }

    public async Task<IReadOnlyList<VisitResponse>> GetVisitsByBuyerIdAsync(
        Guid buyerId,
        CancellationToken cancellationToken = default)
    {
        return await QueryVisits(v => v.BuyerId == buyerId && v.DeletedAt == null, cancellationToken);
    }

    public async Task<IReadOnlyList<VisitResponse>> GetVisitsByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await QueryVisits(v => v.PropertyId == propertyId && v.DeletedAt == null, cancellationToken);
    }

    public async Task<IReadOnlyList<VisitResponse>> GetVisitsByAgentDistrictsAsync(
        IReadOnlyList<short> districtIds,
        CancellationToken cancellationToken = default)
    {
        var visits = await _context.PropertyVisits
            .AsNoTracking()
            .Include(v => v.Property)
            .Include(v => v.Buyer)
            .Include(v => v.AssignedAgent)
            .Where(v => v.DeletedAt == null && districtIds.Contains(v.Property!.DistrictId))
            .OrderByDescending(v => v.ScheduledAt)
            .ToListAsync(cancellationToken);

        return await MapVisitsToResponse(visits, cancellationToken);
    }

    public async Task<IReadOnlyList<VisitResponse>> GetAllVisitsAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var queryable = _context.PropertyVisits
            .AsNoTracking()
            .Include(v => v.Property)
            .Include(v => v.Buyer)
            .Include(v => v.AssignedAgent)
            .Where(v => v.DeletedAt == null);

        if (status.HasValue && status.Value > 0)
        {
            queryable = queryable.Where(v => v.Status == status.Value);
        }

        var visits = await queryable
            .OrderByDescending(v => v.ScheduledAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return await MapVisitsToResponse(visits, cancellationToken);
    }

    private async Task<IReadOnlyList<VisitResponse>> QueryVisits(
        System.Linq.Expressions.Expression<Func<PropertyVisit, bool>> predicate,
        CancellationToken cancellationToken)
    {
        var visits = await _context.PropertyVisits
            .AsNoTracking()
            .Include(v => v.Property)
            .Include(v => v.Buyer)
            .Include(v => v.AssignedAgent)
            .Where(predicate)
            .OrderByDescending(v => v.ScheduledAt)
            .ToListAsync(cancellationToken);

        return await MapVisitsToResponse(visits, cancellationToken);
    }

    private async Task<IReadOnlyList<VisitResponse>> MapVisitsToResponse(
        List<PropertyVisit> visits,
        CancellationToken cancellationToken)
    {
        if (visits.Count == 0) return Array.Empty<VisitResponse>();

        var districtIds = visits.Where(v => v.Property != null).Select(v => v.Property!.DistrictId).Distinct().ToList();
        var cityIds = visits.Where(v => v.Property != null).Select(v => v.Property!.CityId).Distinct().ToList();

        var districts = await _context.Districts
            .AsNoTracking()
            .Where(d => districtIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var cities = await _context.Cities
            .AsNoTracking()
            .Where(c => cityIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        return visits.Select(v =>
        {
            districts.TryGetValue(v.Property?.DistrictId ?? 0, out var districtName);
            cities.TryGetValue(v.Property?.CityId ?? 0, out var cityName);

            return new VisitResponse
            {
                VisitId = v.Id,
                PropertyId = v.PropertyId,
                PropertyTitle = v.Property?.Title ?? "Property Listing",
                PropertyReferenceNo = v.Property?.ReferenceNo ?? string.Empty,
                DistrictName = districtName ?? string.Empty,
                CityName = cityName ?? string.Empty,
                BuyerId = v.BuyerId,
                BuyerName = v.Buyer != null ? $"{v.Buyer.FirstName} {v.Buyer.LastName}".Trim() : "Buyer",
                BuyerEmail = v.Buyer?.Email ?? string.Empty,
                BuyerPhone = v.Buyer?.PhoneNumber ?? string.Empty,
                AssignedAgentId = v.AssignedAgentId,
                AssignedAgentName = v.AssignedAgent != null ? $"{v.AssignedAgent.FirstName} {v.AssignedAgent.LastName}".Trim() : null,
                ScheduledAt = v.ScheduledAt,
                TimeSlot = v.TimeSlot,
                BuyerNotes = v.BuyerNotes,
                AgentNotes = v.AgentNotes,
                Status = v.Status,
                StatusName = ResolveVisitStatusName(v.Status),
                CreatedAt = v.CreatedAt
            };
        }).ToList();
    }

    private static string ResolveVisitStatusName(short status) => status switch
    {
        1 => "Requested",
        2 => "Confirmed",
        3 => "Completed",
        4 => "Cancelled",
        5 => "Rejected",
        _ => $"Status #{status}"
    };
}
