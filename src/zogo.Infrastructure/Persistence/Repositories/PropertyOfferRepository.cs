using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using zogo.Application.DTOs.Offers;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Buyer;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class PropertyOfferRepository : IPropertyOfferRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyOfferRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PropertyOffer offer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(offer);
        await _context.PropertyOffers.AddAsync(offer, cancellationToken);
    }

    public async Task UpdateAsync(PropertyOffer offer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(offer);
        _context.PropertyOffers.Update(offer);
        await Task.CompletedTask;
    }

    public async Task<PropertyOffer?> GetByIdAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyOffers
            .Include(o => o.Property)
            .Include(o => o.Buyer)
            .FirstOrDefaultAsync(o => o.Id == offerId && o.DeletedAt == null, cancellationToken);
    }

    public async Task<OfferResponse?> GetOfferDetailsAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        var offer = await _context.PropertyOffers
            .AsNoTracking()
            .Include(o => o.Property)
            .Include(o => o.Buyer)
            .FirstOrDefaultAsync(o => o.Id == offerId && o.DeletedAt == null, cancellationToken);

        if (offer == null) return null;

        var mapped = await MapOffersToResponse(new List<PropertyOffer> { offer }, cancellationToken);
        return mapped.FirstOrDefault();
    }

    public async Task<IReadOnlyList<OfferResponse>> GetOffersByBuyerIdAsync(
        Guid buyerId,
        CancellationToken cancellationToken = default)
    {
        return await QueryOffers(o => o.BuyerId == buyerId && o.DeletedAt == null, cancellationToken);
    }

    public async Task<IReadOnlyList<OfferResponse>> GetOffersByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await QueryOffers(o => o.PropertyId == propertyId && o.DeletedAt == null, cancellationToken);
    }

    public async Task<IReadOnlyList<OfferResponse>> GetAllOffersAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var queryable = _context.PropertyOffers
            .AsNoTracking()
            .Include(o => o.Property)
            .Include(o => o.Buyer)
            .Where(o => o.DeletedAt == null);

        if (status.HasValue && status.Value > 0)
        {
            queryable = queryable.Where(o => o.Status == status.Value);
        }

        var offers = await queryable
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return await MapOffersToResponse(offers, cancellationToken);
    }

    private async Task<IReadOnlyList<OfferResponse>> QueryOffers(
        Expression<Func<PropertyOffer, bool>> predicate,
        CancellationToken cancellationToken)
    {
        var offers = await _context.PropertyOffers
            .AsNoTracking()
            .Include(o => o.Property)
            .Include(o => o.Buyer)
            .Where(predicate)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return await MapOffersToResponse(offers, cancellationToken);
    }

    private async Task<IReadOnlyList<OfferResponse>> MapOffersToResponse(
        List<PropertyOffer> offers,
        CancellationToken cancellationToken)
    {
        if (offers.Count == 0) return Array.Empty<OfferResponse>();

        var districtIds = offers.Where(o => o.Property != null).Select(o => o.Property!.DistrictId).Distinct().ToList();
        var cityIds = offers.Where(o => o.Property != null).Select(o => o.Property!.CityId).Distinct().ToList();
        var sellerIds = offers.Where(o => o.Property != null).Select(o => o.Property!.CreatedBy).Distinct().ToList();

        var districts = await _context.Districts
            .AsNoTracking()
            .Where(d => districtIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var cities = await _context.Cities
            .AsNoTracking()
            .Where(c => cityIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var sellers = await _context.Users
            .AsNoTracking()
            .Where(u => sellerIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim(), cancellationToken);

        return offers.Select(o =>
        {
            districts.TryGetValue(o.Property?.DistrictId ?? 0, out var districtName);
            cities.TryGetValue(o.Property?.CityId ?? 0, out var cityName);
            sellers.TryGetValue(o.Property?.CreatedBy ?? Guid.Empty, out var sellerName);

            return new OfferResponse
            {
                OfferId = o.Id,
                PropertyId = o.PropertyId,
                PropertyTitle = o.Property?.Title ?? "Property Listing",
                PropertyReferenceNo = o.Property?.ReferenceNo ?? string.Empty,
                AskingPrice = o.Property?.AskingPrice ?? 0,
                DistrictName = districtName ?? string.Empty,
                CityName = cityName ?? string.Empty,
                BuyerId = o.BuyerId,
                BuyerName = o.Buyer != null ? $"{o.Buyer.FirstName} {o.Buyer.LastName}".Trim() : "Buyer",
                BuyerEmail = o.Buyer?.Email ?? string.Empty,
                BuyerPhone = o.Buyer?.PhoneNumber ?? string.Empty,
                SellerId = o.Property?.CreatedBy ?? Guid.Empty,
                SellerName = sellerName ?? "Seller",
                OfferAmount = o.OfferAmount,
                CounterAmount = o.CounterAmount,
                BuyerNotes = o.BuyerNotes,
                SellerNotes = o.SellerNotes,
                ExpiresAt = o.ExpiresAt,
                Status = o.Status,
                StatusName = ResolveOfferStatusName(o.Status),
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };
        }).ToList();
    }

    private static string ResolveOfferStatusName(short status) => status switch
    {
        1 => "Submitted",
        2 => "Countered",
        3 => "Accepted",
        4 => "Rejected",
        5 => "Withdrawn",
        6 => "Expired",
        _ => $"Status #{status}"
    };
}
