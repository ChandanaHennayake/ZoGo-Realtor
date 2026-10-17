using zogo.Application.DTOs.Property.PropertyMedia;

namespace zogo.Application.DTOs.Buyer;

public sealed class BuyerPropertyDetailsResponse
{
    public Guid PropertyId { get; set; }
    public string ReferenceNo { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public short PropertyTypeId { get; set; }
    public string PropertyTypeName { get; set; } = null!;

    public short ListingTypeId { get; set; }
    public string ListingTypeName { get; set; } = null!;

    public decimal AskingPrice { get; set; }
    public bool IsNegotiable { get; set; }

    public short ProvinceId { get; set; }
    public string? ProvinceName { get; set; }

    public short DistrictId { get; set; }
    public string? DistrictName { get; set; }

    public int CityId { get; set; }
    public string? City { get; set; }

    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public short? Bedrooms { get; set; }
    public short? Bathrooms { get; set; }
    public decimal? PropertySize { get; set; }
    public string? SizeUnit { get; set; }

    public short Status { get; set; }
    public string StatusName { get; set; } = null!;

    public bool IsFavorite { get; set; }

    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<PropertyMediaResponse> Media { get; set; } = new();
    public List<BuyerFeatureItem> Features { get; set; } = new();
    public List<BuyerAmenityItem> Amenities { get; set; } = new();
    public BuyerFinancialsItem? Financials { get; set; }
    public BuyerLegalDetailsItem? LegalDetails { get; set; }
}

public sealed class BuyerFeatureItem
{
    public int FeatureId { get; set; }
    public string Name { get; set; } = null!;
    public string? Category { get; set; }
}

public sealed class BuyerAmenityItem
{
    public int AmenityId { get; set; }
    public string Name { get; set; } = null!;
    public string? Category { get; set; }
}

public sealed class BuyerFinancialsItem
{
    public decimal? MaintenanceFee { get; set; }
    public short? MaintenanceFeePeriod { get; set; }
    public decimal? SinkingFundAmount { get; set; }
    public short? SinkingFundPeriod { get; set; }
    public bool? BillsUpToDate { get; set; }
    public bool HasOutstandingCharges { get; set; }
    public decimal? OutstandingAmount { get; set; }
    public string? OutstandingDescription { get; set; }
}

public sealed class BuyerLegalDetailsItem
{
    public short? OwnershipType { get; set; }
    public bool HasMortgage { get; set; }
    public string? MortgageProvider { get; set; }
    public bool HasLegalIssues { get; set; }
    public string? LegalIssueDescription { get; set; }
    public bool LegalVerified { get; set; }
}
