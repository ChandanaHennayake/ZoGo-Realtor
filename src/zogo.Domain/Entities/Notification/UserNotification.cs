using zogo.Domain.Entities.Identity;

namespace zogo.Domain.Entities.Notification;

public class UserNotification
{
    private UserNotification()
    {
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Type { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? ReferenceId { get; private set; }
    public string? ReferenceType { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; } = null!;

    public static UserNotification Create(
        Guid userId,
        string type,
        string title,
        string message,
        string? referenceId = null,
        string? referenceType = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required.", nameof(message));

        return new UserNotification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = string.IsNullOrWhiteSpace(type) ? "GENERAL" : type.Trim().ToUpperInvariant(),
            Title = title.Trim(),
            Message = message.Trim(),
            ReferenceId = referenceId,
            ReferenceType = referenceType,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}
