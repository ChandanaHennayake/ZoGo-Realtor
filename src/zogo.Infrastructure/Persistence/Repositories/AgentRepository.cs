using Microsoft.EntityFrameworkCore;
using zogo.Application.DTOs.Agent;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Agent;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Entities.Master;
using zogo.Domain.Enums;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class AgentRepository : IAgentRepository
{
    private readonly ApplicationDbContext _context;

    public AgentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddProfileAsync(AgentProfile profile, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);
        await _context.AgentProfiles.AddAsync(profile, cancellationToken);
    }

    public async Task AddDistrictsAsync(IEnumerable<AgentDistrict> districts, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(districts);
        await _context.AgentDistricts.AddRangeAsync(districts, cancellationToken);
    }

    public async Task RemoveDistrictsAsync(Guid agentProfileId, CancellationToken cancellationToken = default)
    {
        var existing = await _context.AgentDistricts
            .Where(x => x.AgentId == agentProfileId)
            .ToListAsync(cancellationToken);

        if (existing.Count > 0)
        {
            _context.AgentDistricts.RemoveRange(existing);
        }
    }

    public async Task<AgentProfile?> GetByIdAsync(Guid agentProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.AgentProfiles
            .Include(x => x.User)
            .Include(x => x.City)
            .Include(x => x.AgentDistricts)
                .ThenInclude(x => x.District)
            .FirstOrDefaultAsync(x => x.Id == agentProfileId, cancellationToken);
    }

    public async Task<AgentProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AgentProfiles
            .Include(x => x.User)
            .Include(x => x.City)
            .Include(x => x.AgentDistricts)
                .ThenInclude(x => x.District)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<AgentProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AgentProfiles
            .Include(x => x.User)
            .Include(x => x.City)
            .Include(x => x.AgentDistricts)
                .ThenInclude(x => x.District)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AgentProfile>> GetActiveAgentsByDistrictIdAsync(short districtId, CancellationToken cancellationToken = default)
    {
        return await _context.AgentProfiles
            .Include(x => x.User)
            .Include(x => x.AgentDistricts)
            .Where(x => x.IsActive && x.AgentDistricts.Any(d => d.DistrictId == districtId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AgentProfiles.AnyAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<bool> CityExistsAsync(int cityId, CancellationToken cancellationToken = default)
    {
        return await _context.Cities.AnyAsync(c => c.Id == cityId, cancellationToken);
    }

    public async Task<int> CountActiveDistrictsAsync(IEnumerable<short> districtIds, CancellationToken cancellationToken = default)
    {
        var idList = districtIds.Distinct().ToList();
        return await _context.Districts.CountAsync(d => idList.Contains(d.Id) && d.IsActive, cancellationToken);
    }

    public async Task<AgentDashboardResponse> GetDashboardAsync(
        IReadOnlyList<short> districtIds,
        CancellationToken cancellationToken = default)
    {
        if (districtIds.Count == 0)
        {
            return new AgentDashboardResponse
            {
                InterestedProperties = 0,
                PendingVisits = 0,
                PendingDocuments = 0
            };
        }

        var interestedCount = await (
            from pi in _context.PropertyInterests
            join p in _context.Properties on pi.PropertyId equals p.Id
            where p.Status == 2 &&
                  districtIds.Contains(p.DistrictId) &&
                  pi.Status == (short)PropertyInterestStatus.Interested
            select pi.PropertyId
        ).Distinct().CountAsync(cancellationToken);

        var pendingVisitsCount = await (
            from bi in _context.BuyerPropertyInteractions
            join p in _context.Properties on bi.PropertyId equals p.Id
            where p.Status == 2 &&
                  districtIds.Contains(p.DistrictId) &&
                  bi.CurrentStatus == (short)BuyerPropertyInteractionStatus.VisitRequested
            select bi.Id
        ).CountAsync(cancellationToken);

        var pendingDocsCount = await (
            from bi in _context.BuyerPropertyInteractions
            join p in _context.Properties on bi.PropertyId equals p.Id
            where p.Status == 2 &&
                  districtIds.Contains(p.DistrictId) &&
                  bi.CurrentStatus == (short)BuyerPropertyInteractionStatus.DocumentsRequested
            select bi.Id
        ).CountAsync(cancellationToken);

        return new AgentDashboardResponse
        {
            InterestedProperties = interestedCount,
            PendingVisits = pendingVisitsCount,
            PendingDocuments = pendingDocsCount
        };
    }

    public async Task<IReadOnlyList<AgentInterestedPropertyResponse>> GetInterestedPropertiesAsync(
        IReadOnlyList<short> districtIds,
        CancellationToken cancellationToken = default)
    {
        if (districtIds.Count == 0)
            return [];

        var query =
            from pi in _context.PropertyInterests
            join p in _context.Properties on pi.PropertyId equals p.Id
            join buyer in _context.Users on pi.BuyerId equals buyer.Id
            join seller in _context.Users on p.OwnerUserId equals seller.Id
            join district in _context.Districts on p.DistrictId equals district.Id
            join interaction in _context.BuyerPropertyInteractions
                on new { pi.PropertyId, pi.BuyerId } equals new { interaction.PropertyId, interaction.BuyerId } into interGroup
            from subInteraction in interGroup.DefaultIfEmpty()
            where p.Status == 2 &&
                  districtIds.Contains(p.DistrictId) &&
                  pi.Status == (short)PropertyInterestStatus.Interested
            orderby pi.CreatedAt descending
            select new AgentInterestedPropertyResponse
            {
                PropertyId = p.Id,
                Title = p.Title,
                Price = p.AskingPrice,
                District = new AgentDistrictDto
                {
                    Id = district.Id,
                    Name = district.Name
                },
                Seller = new AgentUserDto
                {
                    UserId = seller.Id,
                    Name = (seller.FirstName + " " + seller.LastName).Trim(),
                    PhoneNumber = seller.PhoneNumber
                },
                Buyer = new AgentUserDto
                {
                    UserId = buyer.Id,
                    Name = (buyer.FirstName + " " + buyer.LastName).Trim(),
                    PhoneNumber = buyer.PhoneNumber
                },
                InterestStatus = pi.Status,
                InteractionStatus = subInteraction != null
                    ? subInteraction.CurrentStatus
                    : (short)BuyerPropertyInteractionStatus.Interested
            };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AgentPendingVisitResponse>> GetPendingVisitsAsync(
        IReadOnlyList<short> districtIds,
        CancellationToken cancellationToken = default)
    {
        if (districtIds.Count == 0)
            return [];

        var query =
            from bi in _context.BuyerPropertyInteractions
            join p in _context.Properties on bi.PropertyId equals p.Id
            join buyer in _context.Users on bi.BuyerId equals buyer.Id
            join seller in _context.Users on p.OwnerUserId equals seller.Id
            join district in _context.Districts on p.DistrictId equals district.Id
            where bi.CurrentStatus == (short)BuyerPropertyInteractionStatus.VisitRequested &&
                  p.Status == 2 &&
                  districtIds.Contains(p.DistrictId)
            orderby p.CreatedAt descending
            select new AgentPendingVisitResponse
            {
                PropertyId = p.Id,
                Title = p.Title,
                ReferenceNo = p.ReferenceNo,
                Price = p.AskingPrice,
                AddressLine1 = p.AddressLine1,
                District = new AgentDistrictDto
                {
                    Id = district.Id,
                    Name = district.Name
                },
                Seller = new AgentUserDto
                {
                    UserId = seller.Id,
                    Name = (seller.FirstName + " " + seller.LastName).Trim(),
                    PhoneNumber = seller.PhoneNumber
                },
                Buyer = new AgentUserDto
                {
                    UserId = buyer.Id,
                    Name = (buyer.FirstName + " " + buyer.LastName).Trim(),
                    PhoneNumber = buyer.PhoneNumber
                },
                InteractionStatus = bi.CurrentStatus
            };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AgentPendingDocumentResponse>> GetPendingDocumentsAsync(
        IReadOnlyList<short> districtIds,
        CancellationToken cancellationToken = default)
    {
        if (districtIds.Count == 0)
            return [];

        var query =
            from bi in _context.BuyerPropertyInteractions
            join p in _context.Properties on bi.PropertyId equals p.Id
            join buyer in _context.Users on bi.BuyerId equals buyer.Id
            join seller in _context.Users on p.OwnerUserId equals seller.Id
            join district in _context.Districts on p.DistrictId equals district.Id
            where bi.CurrentStatus == (short)BuyerPropertyInteractionStatus.DocumentsRequested &&
                  p.Status == 2 &&
                  districtIds.Contains(p.DistrictId)
            orderby p.CreatedAt descending
            select new AgentPendingDocumentResponse
            {
                PropertyId = p.Id,
                Title = p.Title,
                ReferenceNo = p.ReferenceNo,
                Price = p.AskingPrice,
                AddressLine1 = p.AddressLine1,
                District = new AgentDistrictDto
                {
                    Id = district.Id,
                    Name = district.Name
                },
                Seller = new AgentUserDto
                {
                    UserId = seller.Id,
                    Name = (seller.FirstName + " " + seller.LastName).Trim(),
                    PhoneNumber = seller.PhoneNumber
                },
                Buyer = new AgentUserDto
                {
                    UserId = buyer.Id,
                    Name = (buyer.FirstName + " " + buyer.LastName).Trim(),
                    PhoneNumber = buyer.PhoneNumber
                },
                InteractionStatus = bi.CurrentStatus
            };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Property?> GetPropertyByIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties.FirstOrDefaultAsync(p => p.Id == propertyId, cancellationToken);
    }

    public async Task<BuyerPropertyInteraction?> GetInteractionAsync(Guid propertyId, Guid? buyerId, CancellationToken cancellationToken = default)
    {
        if (buyerId.HasValue && buyerId.Value != Guid.Empty)
        {
            return await _context.BuyerPropertyInteractions
                .FirstOrDefaultAsync(bi => bi.PropertyId == propertyId && bi.BuyerId == buyerId.Value, cancellationToken);
        }

        var interaction = await _context.BuyerPropertyInteractions
            .FirstOrDefaultAsync(bi => bi.PropertyId == propertyId &&
                                       bi.CurrentStatus == (short)BuyerPropertyInteractionStatus.VisitRequested,
                                 cancellationToken);

        if (interaction is not null)
            return interaction;

        return await _context.BuyerPropertyInteractions
            .FirstOrDefaultAsync(bi => bi.PropertyId == propertyId, cancellationToken);
    }
}
