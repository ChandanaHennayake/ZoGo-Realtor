namespace zogo.Application.DTOs.Property.PropertyFeature;

public class PropertyFeatureResponse
{
    public Guid PropertyId { get; set; }

    public int FeatureId { get; set; }

    public string FeatureCode { get; set; } = null!;

    public string FeatureName { get; set; } = null!;

    public string? Category { get; set; }

    public int DisplayOrder { get; set; }
}