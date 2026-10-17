using Microsoft.EntityFrameworkCore;
using zogo.Application.DTOs.Admin;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class AdminPropertyRepository : IAdminPropertyRepository
{
    private readonly ApplicationDbContext _context;

    public AdminPropertyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AdminPropertySummaryResponse>> GetPropertiesByStatusAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var queryable = _context.Properties
            .AsNoTracking()
            .Where(p => p.DeletedAt == null);

        if (status.HasValue && status.Value > 0)
        {
            queryable = queryable.Where(p => p.Status == status.Value);
        }

        var properties = await queryable
            .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (properties.Count == 0)
        {
            return Array.Empty<AdminPropertySummaryResponse>();
        }

        var propertyIds = properties.Select(p => p.Id).ToList();
        var ownerIds = properties.Select(p => p.OwnerUserId).Distinct().ToList();

        var users = await _context.Users
            .AsNoTracking()
            .Where(u => ownerIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var propertyTypes = await _context.PropertyTypes
            .AsNoTracking()
            .ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        var listingTypes = await _context.ListingTypes
            .AsNoTracking()
            .ToDictionaryAsync(l => l.Id, l => l.Name, cancellationToken);

        var districts = await _context.Districts
            .AsNoTracking()
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var cities = await _context.Cities
            .AsNoTracking()
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var mediaList = await _context.PropertyMedia
            .AsNoTracking()
            .Where(m => propertyIds.Contains(m.PropertyId))
            .ToListAsync(cancellationToken);

        var docList = await _context.PropertyDocuments
            .AsNoTracking()
            .Where(d => propertyIds.Contains(d.PropertyId))
            .ToListAsync(cancellationToken);

        var result = new List<AdminPropertySummaryResponse>(properties.Count);

        foreach (var p in properties)
        {
            users.TryGetValue(p.OwnerUserId, out var owner);
            propertyTypes.TryGetValue(p.PropertyTypeId, out var pTypeName);
            listingTypes.TryGetValue(p.ListingTypeId, out var lTypeName);
            districts.TryGetValue(p.DistrictId, out var districtName);
            cities.TryGetValue(p.CityId, out var cityName);

            var pMedia = mediaList.Where(m => m.PropertyId == p.Id).ToList();
            var cover = pMedia.FirstOrDefault(m => m.IsCover) ?? pMedia.FirstOrDefault();
            var docCount = docList.Count(d => d.PropertyId == p.Id);

            result.Add(new AdminPropertySummaryResponse
            {
                PropertyId = p.Id,
                ReferenceNo = p.ReferenceNo,
                Title = p.Title,
                PropertyTypeId = p.PropertyTypeId,
                PropertyTypeName = pTypeName ?? "Unknown",
                ListingTypeId = p.ListingTypeId,
                ListingTypeName = lTypeName ?? "Unknown",
                DistrictName = districtName ?? "Unknown",
                CityName = cityName ?? "Unknown",
                AskingPrice = p.AskingPrice,
                Status = p.Status,
                StatusName = ResolveStatusName(p.Status),
                OwnerUserId = p.OwnerUserId,
                OwnerName = owner != null ? $"{owner.FirstName} {owner.LastName}".Trim() : "Unknown",
                OwnerEmail = owner?.Email ?? string.Empty,
                PhotoCount = pMedia.Count,
                DocumentCount = docCount,
                PrimaryImageUrl = cover != null ? $"/api/v1/properties/{p.Id}/media/{cover.Id}/download" : null,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        return result;
    }

    public async Task<AdminPropertyReviewDetailsResponse?> GetPropertyReviewDetailsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == propertyId && p.DeletedAt == null, cancellationToken);

        if (property == null)
            return null;

        var owner = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == property.OwnerUserId, cancellationToken);

        var pType = await _context.PropertyTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == property.PropertyTypeId, cancellationToken);

        var lType = await _context.ListingTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == property.ListingTypeId, cancellationToken);

        var province = await _context.Provinces
            .AsNoTracking()
            .FirstOrDefaultAsync(pr => pr.Id == property.ProvinceId, cancellationToken);

        var district = await _context.Districts
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == property.DistrictId, cancellationToken);

        var city = await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == property.CityId, cancellationToken);

        var media = await _context.PropertyMedia
            .AsNoTracking()
            .Where(m => m.PropertyId == propertyId)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new AdminPropertyMediaItem
            {
                MediaId = m.Id,
                MediaUrl = $"/api/v1/properties/{propertyId}/media/{m.Id}/download",
                IsCover = m.IsCover,
                DisplayOrder = m.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        var docs = await _context.PropertyDocuments
            .AsNoTracking()
            .Where(d => d.PropertyId == propertyId)
            .Select(d => new AdminPropertyDocumentItem
            {
                DocumentId = d.Id,
                DocumentType = d.DocumentTypeId.ToString(),
                DocumentName = d.OriginalFileName ?? "Document",
                DocumentUrl = $"/api/v1/properties/{propertyId}/documents/{d.Id}/download",
                UploadedAt = d.UploadedAt
            })
            .ToListAsync(cancellationToken);

        var apt = await _context.ApartmentDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.PropertyId == propertyId, cancellationToken);

        var amenities = await (
            from pa in _context.PropertyAmenities
            join am in _context.Amenities on pa.AmenityId equals am.Id
            where pa.PropertyId == propertyId
            select am.Name
        ).ToListAsync(cancellationToken);

        var features = await (
            from pf in _context.PropertyFeatures
            join f in _context.Features on pf.FeatureId equals f.Id
            where pf.PropertyId == propertyId
            select f.Name
        ).ToListAsync(cancellationToken);

        return new AdminPropertyReviewDetailsResponse
        {
            PropertyId = property.Id,
            ReferenceNo = property.ReferenceNo,
            Title = property.Title,
            Description = property.Description,
            PropertyTypeId = property.PropertyTypeId,
            PropertyTypeName = pType?.Name ?? "Unknown",
            ListingTypeId = property.ListingTypeId,
            ListingTypeName = lType?.Name ?? "Unknown",
            ProvinceName = province?.Name ?? "Unknown",
            DistrictName = district?.Name ?? "Unknown",
            CityName = city?.Name ?? "Unknown",
            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            PostalCode = property.PostalCode,
            Latitude = property.Latitude,
            Longitude = property.Longitude,
            AskingPrice = property.AskingPrice,
            IsNegotiable = property.IsNegotiable,
            Status = property.Status,
            StatusName = ResolveStatusName(property.Status),
            CreatedAt = property.CreatedAt,
            OwnerUserId = property.OwnerUserId,
            OwnerName = owner != null ? $"{owner.FirstName} {owner.LastName}".Trim() : "Unknown",
            OwnerEmail = owner?.Email ?? string.Empty,
            OwnerPhone = owner?.PhoneNumber ?? string.Empty,
            Media = media,
            Documents = docs,
            Bedrooms = apt?.Bedrooms,
            Bathrooms = apt?.Bathrooms,
            FloorAreaSqFt = apt?.FloorAreaSqFt,
            Amenities = amenities,
            Features = features
        };
    }

    public async Task<Property?> GetByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == propertyId && p.DeletedAt == null, cancellationToken);
    }

    public async Task<AdminDashboardStatsResponse> GetDashboardStatsAsync(
        CancellationToken cancellationToken = default)
    {
        var totalProps = await _context.Properties.CountAsync(p => p.DeletedAt == null, cancellationToken);
        var pendingCount = await _context.Properties.CountAsync(p => p.Status == 4 && p.DeletedAt == null, cancellationToken);
        var reviewCount = await _context.Properties.CountAsync(p => p.Status == 3 && p.DeletedAt == null, cancellationToken);
        var changesCount = await _context.Properties.CountAsync(p => p.Status == 9 && p.DeletedAt == null, cancellationToken);
        var publishedCount = await _context.Properties.CountAsync(p => p.Status == 2 && p.DeletedAt == null, cancellationToken);
        var rejectedCount = await _context.Properties.CountAsync(p => p.Status == 10 && p.DeletedAt == null, cancellationToken);
        var soldCount = await _context.Properties.CountAsync(p => p.Status == 5 && p.DeletedAt == null, cancellationToken);

        var totalUsers = await _context.Users.CountAsync(u => u.DeletedAt == null, cancellationToken);
        var totalAgents = await _context.AgentProfiles.CountAsync(cancellationToken);
        var activeAgents = await _context.AgentProfiles.CountAsync(a => a.IsActive, cancellationToken);
        var districtsCovered = await _context.AgentDistricts.Select(ad => ad.DistrictId).Distinct().CountAsync(cancellationToken);

        return new AdminDashboardStatsResponse
        {
            TotalProperties = totalProps,
            PendingApprovalsCount = pendingCount,
            UnderReviewCount = reviewCount,
            ChangesRequestedCount = changesCount,
            PublishedCount = publishedCount,
            RejectedCount = rejectedCount,
            SoldCount = soldCount,
            TotalUsers = totalUsers,
            TotalAgents = totalAgents,
            ActiveAgents = activeAgents,
            TotalDistrictsCovered = districtsCovered
        };
    }

    private static string ResolveStatusName(short status) => status switch
    {
        1 => "Draft",
        2 => "Published",
        3 => "Under Review",
        4 => "Pending Approval",
        5 => "Sold",
        6 => "Rented",
        7 => "Suspended",
        8 => "Inactive",
        9 => "Changes Requested",
        10 => "Rejected",
        _ => $"Status #{status}"
    };
}
