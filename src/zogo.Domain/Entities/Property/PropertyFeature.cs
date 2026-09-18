using zogo.Domain.Entities.Master;

namespace zogo.Domain.Entities.Property;

public class PropertyFeature
{
    private PropertyFeature()
    {
    }

    public Guid PropertyId { get; private set; }

    public int FeatureId { get; private set; }

    public Feature Feature { get; private set; } = null!;

    public static PropertyFeature Create(
        Guid propertyId,
        int featureId)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        if (featureId <= 0)
            throw new ArgumentException(
                "Feature ID is required.",
                nameof(featureId));

        return new PropertyFeature
        {
            PropertyId = propertyId,
            FeatureId = featureId
        };
    }
}