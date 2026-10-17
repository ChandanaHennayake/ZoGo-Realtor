namespace zogo.Application.DTOs.Buyer;

public sealed class BuyerPropertyResponse
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

    public string? ImageUrl { get; set; }
    public List<string> Images { get; set; } = new();

    public short Status { get; set; }
    public string StatusName { get; set; } = null!;

    public bool IsFavorite { get; set; }

    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
