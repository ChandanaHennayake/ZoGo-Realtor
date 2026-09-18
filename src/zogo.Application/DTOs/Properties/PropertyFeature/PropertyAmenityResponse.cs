namespace zogo.Application.DTOs.Property.PropertyAmenity;

public class PropertyAmenityResponse
{
    public Guid PropertyId { get; set; }

    public int AmenityId { get; set; }

    public string AmenityCode { get; set; } = null!;

    public string AmenityName { get; set; } = null!;

    public string? Category { get; set; }

    public int DisplayOrder { get; set; }
}