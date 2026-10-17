using zogo.Domain.Entities.Agent;
using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;

namespace zogo.Domain.Entities.Notification;

public class AgentNotification
{
    private AgentNotification()
    {
    }

    public Guid Id { get; private set; }
    public Guid AgentProfileId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid BuyerId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AgentProfile AgentProfile { get; private set; } = null!;
    public Domain.Entities.Master.Property Property { get; private set; } = null!;
    public User Buyer { get; private set; } = null!;

    public static AgentNotification Create(
        Guid agentProfileId,
        Guid propertyId,
        Guid buyerId,
        string title,
        string message)
    {
        if (agentProfileId == Guid.Empty)
            throw new ArgumentException("Agent profile ID is required.", nameof(agentProfileId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required.", nameof(message));

        return new AgentNotification
        {
            Id = Guid.NewGuid(),
            AgentProfileId = agentProfileId,
            PropertyId = propertyId,
            BuyerId = buyerId,
            Title = title.Trim(),
            Message = message.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
