using zogo.Application.DTOs.Notification;

namespace zogo.Application.Interfaces.Services;

public interface IUserNotificationService
{
    Task<IReadOnlyList<UserNotificationResponse>> GetUserNotificationsAsync(
        Guid userId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    Task<UnreadCountResponse> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<UserNotificationResponse> SendNotificationAsync(
        SendNotificationRequest request,
        CancellationToken cancellationToken = default);
}
