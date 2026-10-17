using Microsoft.EntityFrameworkCore;
using zogo.Application.DTOs.Buyer;
using zogo.Application.DTOs.Property.PropertyMedia;
using zogo.Application.Interfaces.Repositories;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class BuyerPropertyRepository : IBuyerPropertyRepository
{
    private readonly ApplicationDbContext _context;

    public BuyerPropertyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BuyerPropertyResponse>> GetPublishedPropertiesAsync(
        BuyerPropertyListQuery query,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var queryable = _context.Properties
            .AsNoTracking()
            .Where(p => p.Status == 2 && p.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchLower = query.Search.Trim().ToLower();
            queryable = queryable.Where(p =>
                p.Title.ToLower().Contains(searchLower) ||
                (p.Description != null && p.Description.ToLower().Contains(searchLower)) ||
                p.AddressLine1.ToLower().Contains(searchLower));
        }

        if (query.PropertyTypeId.HasValue && query.PropertyTypeId.Value > 0)
        {
            queryable = queryable.Where(p => p.PropertyTypeId == query.PropertyTypeId.Value);
        }

        if (query.ListingTypeId.HasValue && query.ListingTypeId.Value > 0)
        {
            queryable = queryable.Where(p => p.ListingTypeId == query.ListingTypeId.Value);
        }

        if (query.DistrictId.HasValue && query.DistrictId.Value > 0)
        {
            queryable = queryable.Where(p => p.DistrictId == query.DistrictId.Value);
        }

        if (query.CityId.HasValue && query.CityId.Value > 0)
        {
            queryable = queryable.Where(p => p.CityId == query.CityId.Value);
        }

        if (query.MinPrice.HasValue && query.MinPrice.Value >= 0)
        {
            queryable = queryable.Where(p => p.AskingPrice >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue && query.MaxPrice.Value > 0)
        {
            queryable = queryable.Where(p => p.AskingPrice <= query.MaxPrice.Value);
        }

        var properties = await queryable
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        if (properties.Count == 0)
        {
            return Array.Empty<BuyerPropertyResponse>();
        }

        var propertyIds = properties.Select(p => p.Id).ToList();

        var apartmentDetails = await _context.ApartmentDetails
            .AsNoTracking()
            .Where(a => propertyIds.Contains(a.PropertyId))
            .ToDictionaryAsync(a => a.PropertyId, cancellationToken);

        var mediaList = await _context.PropertyMedia
            .AsNoTracking()
            .Where(m => propertyIds.Contains(m.PropertyId))
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);

        var districts = await _context.Districts
            .AsNoTracking()
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var cities = await _context.Cities
            .AsNoTracking()
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var provinces = await _context.Provinces
            .AsNoTracking()
            .ToDictionaryAsync(p => p.Id, p => p.Name, cancellationToken);

        var propertyTypes = await _context.PropertyTypes
            .AsNoTracking()
            .ToDictionaryAsync(pt => pt.Id, pt => pt.Name, cancellationToken);

        var listingTypes = await _context.ListingTypes
            .AsNoTracking()
            .ToDictionaryAsync(lt => lt.Id, lt => lt.Name, cancellationToken);

        var favoritePropertyIds = new HashSet<Guid>();
        if (userId.HasValue && userId.Value != Guid.Empty)
        {
            var favs = await _context.Favorites
                .AsNoTracking()
                .Where(f => f.UserId == userId.Value && propertyIds.Contains(f.PropertyId))
                .Select(f => f.PropertyId)
                .ToListAsync(cancellationToken);

            favoritePropertyIds = new HashSet<Guid>(favs);
        }

        var results = new List<BuyerPropertyResponse>(properties.Count);

        foreach (var p in properties)
        {
            apartmentDetails.TryGetValue(p.Id, out var apt);
            var pMedia = mediaList.Where(m => m.PropertyId == p.Id).ToList();
            var cover = pMedia.FirstOrDefault(m => m.IsCover) ?? pMedia.FirstOrDefault();
            var coverUrl = cover != null
                ? $"/api/v1/properties/{p.Id}/media/{cover.Id}/download"
                : null;

            results.Add(new BuyerPropertyResponse
            {
                PropertyId = p.Id,
                ReferenceNo = p.ReferenceNo,
                Title = p.Title,
                Description = p.Description,
                PropertyTypeId = p.PropertyTypeId,
                PropertyTypeName = propertyTypes.TryGetValue(p.PropertyTypeId, out var ptName) ? ptName : "Property",
                ListingTypeId = p.ListingTypeId,
                ListingTypeName = listingTypes.TryGetValue(p.ListingTypeId, out var ltName) ? ltName : (p.ListingTypeId == 2 ? "For Rent" : "For Sale"),
                AskingPrice = p.AskingPrice,
                IsNegotiable = p.IsNegotiable,
                ProvinceId = p.ProvinceId,
                ProvinceName = provinces.TryGetValue(p.ProvinceId, out var provName) ? provName : null,
                DistrictId = p.DistrictId,
                DistrictName = districts.TryGetValue(p.DistrictId, out var distName) ? distName : null,
                CityId = p.CityId,
                City = cities.TryGetValue(p.CityId, out var cityName) ? cityName : null,
                AddressLine1 = p.AddressLine1,
                AddressLine2 = p.AddressLine2,
                PostalCode = p.PostalCode,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Bedrooms = apt?.Bedrooms,
                Bathrooms = apt?.Bathrooms,
                PropertySize = apt?.FloorAreaSqFt,
                SizeUnit = apt != null ? "sq.ft" : null,
                ImageUrl = coverUrl,
                Images = pMedia.Select(m => $"/api/v1/properties/{p.Id}/media/{m.Id}/download").ToList(),
                Status = p.Status,
                StatusName = "Published",
                IsFavorite = favoritePropertyIds.Contains(p.Id),
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt
            });
        }

        return results;
    }

    public async Task<BuyerPropertyDetailsResponse?> GetPublishedPropertyDetailsAsync(
        Guid propertyId,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var p = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == propertyId && x.Status == 2 && x.DeletedAt == null,
                cancellationToken);

        if (p is null)
        {
            return null;
        }

        var apt = await _context.ApartmentDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.PropertyId == propertyId, cancellationToken);

        var media = await _context.PropertyMedia
            .AsNoTracking()
            .Where(m => m.PropertyId == propertyId)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);

        var propertyFeatures = await _context.PropertyFeatures
            .AsNoTracking()
            .Include(pf => pf.Feature)
            .Where(pf => pf.PropertyId == propertyId)
            .ToListAsync(cancellationToken);

        var propertyAmenities = await _context.PropertyAmenities
            .AsNoTracking()
            .Include(pa => pa.Amenity)
            .Where(pa => pa.PropertyId == propertyId)
            .ToListAsync(cancellationToken);

        var financials = await _context.PropertyFinancials
            .AsNoTracking()
            .FirstOrDefaultAsync(pf => pf.PropertyId == propertyId, cancellationToken);

        var legal = await _context.PropertyLegalDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(pl => pl.PropertyId == propertyId, cancellationToken);

        var district = await _context.Districts
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == p.DistrictId, cancellationToken);

        var city = await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == p.CityId, cancellationToken);

        var province = await _context.Provinces
            .AsNoTracking()
            .FirstOrDefaultAsync(pr => pr.Id == p.ProvinceId, cancellationToken);

        var propertyType = await _context.PropertyTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(pt => pt.Id == p.PropertyTypeId, cancellationToken);

        var listingType = await _context.ListingTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(lt => lt.Id == p.ListingTypeId, cancellationToken);

        var isFavorite = false;
        if (userId.HasValue && userId.Value != Guid.Empty)
        {
            isFavorite = await _context.Favorites
                .AsNoTracking()
                .AnyAsync(f => f.UserId == userId.Value && f.PropertyId == propertyId, cancellationToken);
        }

        return new BuyerPropertyDetailsResponse
        {
            PropertyId = p.Id,
            ReferenceNo = p.ReferenceNo,
            Title = p.Title,
            Description = p.Description,
            PropertyTypeId = p.PropertyTypeId,
            PropertyTypeName = propertyType?.Name ?? "Property",
            ListingTypeId = p.ListingTypeId,
            ListingTypeName = listingType?.Name ?? (p.ListingTypeId == 2 ? "For Rent" : "For Sale"),
            AskingPrice = p.AskingPrice,
            IsNegotiable = p.IsNegotiable,
            ProvinceId = p.ProvinceId,
            ProvinceName = province?.Name,
            DistrictId = p.DistrictId,
            DistrictName = district?.Name,
            CityId = p.CityId,
            City = city?.Name,
            AddressLine1 = p.AddressLine1,
            AddressLine2 = p.AddressLine2,
            PostalCode = p.PostalCode,
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            Bedrooms = apt?.Bedrooms,
            Bathrooms = apt?.Bathrooms,
            PropertySize = apt?.FloorAreaSqFt,
            SizeUnit = apt != null ? "sq.ft" : null,
            Status = p.Status,
            StatusName = "Published",
            IsFavorite = isFavorite,
            PublishedAt = p.PublishedAt,
            CreatedAt = p.CreatedAt,
            Media = media.Select(m => new PropertyMediaResponse
            {
                Id = m.Id,
                PropertyId = m.PropertyId,
                MediaType = m.MediaType,
                StorageKey = m.StorageKey,
                OriginalFileName = m.OriginalFileName,
                MimeType = m.MimeType,
                FileSizeBytes = m.FileSizeBytes,
                DisplayOrder = m.DisplayOrder,
                IsCover = m.IsCover,
                CreatedBy = m.CreatedBy,
                CreatedAt = m.CreatedAt
            }).ToList(),
            Features = propertyFeatures.Select(pf => new BuyerFeatureItem
            {
                FeatureId = pf.FeatureId,
                Name = pf.Feature.Name,
                Category = pf.Feature.Category
            }).ToList(),
            Amenities = propertyAmenities.Select(pa => new BuyerAmenityItem
            {
                AmenityId = pa.AmenityId,
                Name = pa.Amenity.Name,
                Category = pa.Amenity.Category
            }).ToList(),
            Financials = financials == null ? null : new BuyerFinancialsItem
            {
                MaintenanceFee = financials.MaintenanceFee,
                MaintenanceFeePeriod = financials.MaintenanceFeePeriod,
                SinkingFundAmount = financials.SinkingFundAmount,
                SinkingFundPeriod = financials.SinkingFundPeriod,
                BillsUpToDate = financials.BillsUpToDate,
                HasOutstandingCharges = financials.HasOutstandingCharges,
                OutstandingAmount = financials.OutstandingAmount,
                OutstandingDescription = financials.OutstandingDescription
            },
            LegalDetails = legal == null ? null : new BuyerLegalDetailsItem
            {
                OwnershipType = legal.OwnershipType,
                HasMortgage = legal.HasMortgage,
                MortgageProvider = legal.MortgageProvider,
                HasLegalIssues = legal.HasLegalIssues,
                LegalIssueDescription = legal.LegalIssueDescription,
                LegalVerified = legal.LegalVerified
            }
        };
    }
}
