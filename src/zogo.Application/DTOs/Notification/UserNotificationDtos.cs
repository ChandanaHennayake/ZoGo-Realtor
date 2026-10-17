namespace zogo.Application.DTOs.Notification;

public record UserNotificationResponse(
    Guid Id,
    Guid UserId,
    string Type,
    string Title,
    string Message,
    string? ReferenceId,
    string? ReferenceType,
    bool IsRead,
    DateTime? ReadAt,
    DateTime CreatedAt);

public record SendNotificationRequest(
    Guid UserId,
    string Type,
    string Title,
    string Message,
    string? ReferenceId = null,
    string? ReferenceType = null);

public record UnreadCountResponse(int UnreadCount);
