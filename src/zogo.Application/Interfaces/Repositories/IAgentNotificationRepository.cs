using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;
using zogo.Domain.Entities.Notification;

namespace zogo.Application.Interfaces.Repositories;

public interface IAgentNotificationRepository
{
    Task AddAsync(AgentNotification notification, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid agentProfileId, Guid propertyId, Guid buyerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentNotification>> GetByAgentProfileIdAsync(Guid agentProfileId, CancellationToken cancellationToken = default);
    Task<Property?> GetPublishedPropertyByIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
