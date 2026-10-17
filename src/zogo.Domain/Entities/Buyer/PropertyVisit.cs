using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;

namespace zogo.Domain.Entities.Buyer;

public class PropertyVisit
{
    private PropertyVisit()
    {
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid? AssignedAgentId { get; private set; }

    public DateTime ScheduledAt { get; private set; }
    public string? TimeSlot { get; private set; }
    public string? BuyerNotes { get; private set; }
    public string? AgentNotes { get; private set; }

    // 1: Requested, 2: Confirmed, 3: Completed, 4: Cancelled, 5: Rejected
    public short Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public Domain.Entities.Master.Property? Property { get; private set; }
    public User? Buyer { get; private set; }
    public User? AssignedAgent { get; private set; }

    public static PropertyVisit Create(
        Guid propertyId,
        Guid buyerId,
        DateTime scheduledAt,
        string? timeSlot,
        string? buyerNotes)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        var scheduledAtUtc = scheduledAt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(scheduledAt, DateTimeKind.Utc)
            : scheduledAt.ToUniversalTime();

        if (scheduledAtUtc.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Visit date cannot be in the past.", nameof(scheduledAt));

        return new PropertyVisit
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            BuyerId = buyerId,
            ScheduledAt = scheduledAtUtc,
            TimeSlot = string.IsNullOrWhiteSpace(timeSlot) ? null : timeSlot.Trim(),
            BuyerNotes = string.IsNullOrWhiteSpace(buyerNotes) ? null : buyerNotes.Trim(),
            Status = 1, // Requested
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Confirm(Guid? agentId, string? notes)
    {
        if (Status != 1)
            throw new InvalidOperationException($"Cannot confirm visit with current status #{Status}.");

        Status = 2; // Confirmed
        if (agentId.HasValue && agentId.Value != Guid.Empty)
            AssignedAgentId = agentId;
        if (!string.IsNullOrWhiteSpace(notes))
            AgentNotes = notes.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string? notes)
    {
        if (Status != 2 && Status != 1)
            throw new InvalidOperationException($"Cannot complete visit with current status #{Status}.");

        Status = 3; // Completed
        if (!string.IsNullOrWhiteSpace(notes))
            AgentNotes = notes.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? reason)
    {
        if (Status == 3)
            throw new InvalidOperationException("Cannot cancel a completed visit.");

        Status = 4; // Cancelled
        if (!string.IsNullOrWhiteSpace(reason))
            AgentNotes = $"Cancelled: {reason.Trim()}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string? reason)
    {
        Status = 5; // Rejected
        if (!string.IsNullOrWhiteSpace(reason))
            AgentNotes = $"Rejected: {reason.Trim()}";
        UpdatedAt = DateTime.UtcNow;
    }
}
