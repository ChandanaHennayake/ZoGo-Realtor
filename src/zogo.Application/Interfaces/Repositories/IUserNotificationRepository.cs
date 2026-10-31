using zogo.Domain.Entities.Notification;

namespace zogo.Application.Interfaces.Repositories;

public interface IUserNotificationRepository
{
    Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserNotification>> GetByUserIdAsync(Guid userId, int limit = 50, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserNotification notification, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserNotification?> GetLatestByReferenceAsync(string referenceId, string? notificationType = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, string>> GetLatestNotesForReferencesAsync(IEnumerable<string> referenceIds, CancellationToken cancellationToken = default);
}
