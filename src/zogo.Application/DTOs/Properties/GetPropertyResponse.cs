namespace zogo.Application.DTOs.Properties;

public sealed class GetPropertyResponse
{
    public Guid PropertyId { get; set; }
    public string ReferenceNo { get; set; } = null!;

    public Guid OwnerUserId { get; set; }

    public short PropertyTypeId { get; set; }
    public short ListingTypeId { get; set; }

    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public short ProvinceId { get; set; }
    public short DistrictId { get; set; }
    public int? DivisionalSecretariatId { get; set; }
    public int? GnDivisionId { get; set; }

    public int CityId { get; set; }
    public string? City { get; set; }

    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public decimal AskingPrice { get; set; }
    public bool IsNegotiable { get; set; }

    public short Status { get; set; }
    public string? StatusName { get; set; }

    public DateTime? PublishedAt { get; set; }
    public DateTime? SoldAt { get; set; }

    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

