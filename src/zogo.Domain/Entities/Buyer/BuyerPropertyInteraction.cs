using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;
using zogo.Domain.Enums;

namespace zogo.Domain.Entities.Buyer;

public class BuyerPropertyInteraction
{
    private BuyerPropertyInteraction()
    {
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid BuyerId { get; private set; }
    public short CurrentStatus { get; private set; }

    public Domain.Entities.Master.Property? Property { get; private set; }
    public User? Buyer { get; private set; }

    public static BuyerPropertyInteraction Create(Guid propertyId, Guid buyerId, short currentStatus)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        ValidateCurrentStatus(currentStatus);

        return new BuyerPropertyInteraction
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            BuyerId = buyerId,
            CurrentStatus = currentStatus
        };
    }

    public void UpdateCurrentStatus(short currentStatus)
    {
        ValidateCurrentStatus(currentStatus);

        CurrentStatus = currentStatus;
    }

    private static void ValidateCurrentStatus(short currentStatus)
    {
        if (!Enum.IsDefined(typeof(BuyerPropertyInteractionStatus), currentStatus))
        {
            throw new ArgumentException(
                $"Invalid buyer property interaction status: {currentStatus}. Allowed values are 1 to 9.",
                nameof(currentStatus));
        }
    }
}
