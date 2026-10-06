using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;
using zogo.Domain.Enums;

namespace zogo.Domain.Entities.Buyer;

public class PropertyInterest
{
    private PropertyInterest()
    {
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid BuyerId { get; private set; }
    public short Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Domain.Entities.Master.Property? Property { get; private set; }
    public User? Buyer { get; private set; }

    public static PropertyInterest Create(Guid propertyId, Guid buyerId, short status)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        ValidateStatus(status);

        return new PropertyInterest
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            BuyerId = buyerId,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(short status)
    {
        ValidateStatus(status);

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateStatus(short status)
    {
        if (!Enum.IsDefined(typeof(PropertyInterestStatus), status))
        {
            throw new ArgumentException(
                $"Invalid property interest status: {status}. Allowed values are 1 (INTERESTED), 2 (CONSIDERING), 3 (NOT_INTERESTED).",
                nameof(status));
        }
    }
}
