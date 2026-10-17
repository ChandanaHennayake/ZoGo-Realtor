using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;
using zogo.Domain.Entities.Notification;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class AgentNotificationRepository : IAgentNotificationRepository
{
    private readonly ApplicationDbContext _context;

    public AgentNotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AgentNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);
        await _context.AgentNotifications.AddAsync(notification, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid agentProfileId, Guid propertyId, Guid buyerId, CancellationToken cancellationToken = default)
    {
        return await _context.AgentNotifications
            .AnyAsync(x => x.AgentProfileId == agentProfileId &&
                           x.PropertyId == propertyId &&
                           x.BuyerId == buyerId,
                      cancellationToken);
    }

    public async Task<IReadOnlyList<AgentNotification>> GetByAgentProfileIdAsync(Guid agentProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.AgentNotifications
            .Include(x => x.Property)
            .Include(x => x.Buyer)
            .Where(x => x.AgentProfileId == agentProfileId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Property?> GetPublishedPropertyByIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties.FirstOrDefaultAsync(p => p.Id == propertyId && p.Status == 2, cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}
