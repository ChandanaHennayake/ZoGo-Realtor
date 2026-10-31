using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Notification;

namespace zogo.Infrastructure.Persistence.Repositories;

public class UserNotificationRepository : IUserNotificationRepository
{
    private readonly ApplicationDbContext _context;

    public UserNotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.UserNotifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<UserNotification>> GetByUserIdAsync(
        Guid userId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserNotifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserNotifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }

    public async Task AddAsync(UserNotification notification, CancellationToken cancellationToken = default)
    {
        await _context.UserNotifications.AddAsync(notification, cancellationToken);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unread = await _context.UserNotifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var item in unread)
        {
            item.MarkAsRead();
        }
    }

    public async Task<UserNotification?> GetLatestByReferenceAsync(
        string referenceId,
        string? notificationType = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.UserNotifications
            .AsNoTracking()
            .Where(n => n.ReferenceId == referenceId);

        if (!string.IsNullOrWhiteSpace(notificationType))
        {
            query = query.Where(n => n.Type == notificationType.Trim().ToUpperInvariant());
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, string>> GetLatestNotesForReferencesAsync(
        IEnumerable<string> referenceIds,
        CancellationToken cancellationToken = default)
    {
        var refList = referenceIds.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList();
        if (refList.Count == 0)
            return new Dictionary<string, string>();

        var notifications = await _context.UserNotifications
            .AsNoTracking()
            .Where(n => n.ReferenceId != null && refList.Contains(n.ReferenceId) &&
                        (n.Type == "CHANGES_REQUESTED" || n.Type == "PROPERTY_REJECTED"))
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        var dict = new Dictionary<string, string>();
        foreach (var n in notifications)
        {
            if (n.ReferenceId != null && !dict.ContainsKey(n.ReferenceId))
            {
                dict[n.ReferenceId] = n.Message;
            }
        }

        return dict;
    }
}
