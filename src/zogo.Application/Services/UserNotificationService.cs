using zogo.Application.DTOs.Notification;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Notification;

namespace zogo.Application.Services;

public class UserNotificationService : IUserNotificationService
{
    private readonly IUserNotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserNotificationService(
        IUserNotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<UserNotificationResponse>> GetUserNotificationsAsync(
        Guid userId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId, limit, cancellationToken);
        return notifications.Select(MapToResponse).ToList();
    }

    public async Task<UnreadCountResponse> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var count = await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);
        return new UnreadCountResponse(count);
    }

    public async Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null || notification.UserId != userId)
        {
            return false;
        }

        notification.MarkAsRead();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserNotificationResponse> SendNotificationAsync(
        SendNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var notification = UserNotification.Create(
            request.UserId,
            request.Type,
            request.Title,
            request.Message,
            request.ReferenceId,
            request.ReferenceType);

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(notification);
    }

    private static UserNotificationResponse MapToResponse(UserNotification n)
    {
        return new UserNotificationResponse(
            n.Id,
            n.UserId,
            n.Type,
            n.Title,
            n.Message,
            n.ReferenceId,
            n.ReferenceType,
            n.IsRead,
            n.ReadAt,
            n.CreatedAt);
    }
}
