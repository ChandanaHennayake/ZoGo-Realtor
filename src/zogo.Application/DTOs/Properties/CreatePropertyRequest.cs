namespace zogo.Application.DTOs.Properties;

public sealed class CreatePropertyRequest
{
    public string ReferenceNo { get; set; } = null!;

    public short PropertyTypeId { get; set; }

    public short ListingTypeId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public short DistrictId { get; set; }

    public int? DivisionalSecretariatId { get; set; }

    public int? GnDivisionId { get; set; }

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = null!;

    public string? PostalCode { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public decimal AskingPrice { get; set; }

    public bool IsNegotiable { get; set; }
}