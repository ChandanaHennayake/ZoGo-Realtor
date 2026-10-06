using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;

namespace zogo.Domain.Entities.Buyer;

public class Favorite
{
    private Favorite()
    {
    }

    public Guid UserId { get; private set; }
    public Guid PropertyId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User? User { get; private set; }
    public Domain.Entities.Master.Property? Property { get; private set; }

    public static Favorite Create(Guid userId, Guid propertyId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        return new Favorite
        {
            UserId = userId,
            PropertyId = propertyId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
