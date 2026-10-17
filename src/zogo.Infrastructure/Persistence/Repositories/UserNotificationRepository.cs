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
}
