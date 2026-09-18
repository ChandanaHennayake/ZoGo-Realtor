using zogo.Domain.Entities.Master;

namespace zogo.Domain.Entities.Property;

public class PropertyAmenity
{
    private PropertyAmenity()
    {
    }

    public Guid PropertyId { get; private set; }

    public int AmenityId { get; private set; }

    public Amenity Amenity { get; private set; } = null!;

    public static PropertyAmenity Create(
        Guid propertyId,
        int amenityId)
    {
        if (propertyId == Guid.Empty)
        {
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));
        }

        if (amenityId <= 0)
        {
            throw new ArgumentException(
                "Amenity ID is required.",
                nameof(amenityId));
        }

        return new PropertyAmenity
        {
            PropertyId = propertyId,
            AmenityId = amenityId
        };
    }
}