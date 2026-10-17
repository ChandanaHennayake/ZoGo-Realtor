using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Notification;

namespace zogo.Application.Services;

public sealed class AgentNotificationService : IAgentNotificationService
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAgentNotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AgentNotificationService(
        IAgentRepository agentRepository,
        IAgentNotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _agentRepository = agentRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task NotifyAgentsOnBuyerInterestAsync(
        Guid propertyId,
        Guid buyerId,
        CancellationToken cancellationToken = default)
    {
        var property = await _notificationRepository.GetPublishedPropertyByIdAsync(propertyId, cancellationToken);
        if (property is null)
            return;

        var buyer = await _notificationRepository.GetUserByIdAsync(buyerId, cancellationToken);
        if (buyer is null)
            return;

        var agents = await _agentRepository.GetActiveAgentsByDistrictIdAsync(property.DistrictId, cancellationToken);
        if (agents.Count == 0)
            return;

        var title = "New Buyer Interest";
        var buyerName = $"{buyer.FirstName} {buyer.LastName}".Trim();
        var message = $"A buyer ({buyerName}) has shown interest in property '{property.Title}' in your assigned district.";

        var notificationsAdded = false;

        foreach (var agent in agents)
        {
            var exists = await _notificationRepository.ExistsAsync(agent.Id, propertyId, buyerId, cancellationToken);
            if (!exists)
            {
                var notification = AgentNotification.Create(
                    agent.Id,
                    propertyId,
                    buyerId,
                    title,
                    message);

                await _notificationRepository.AddAsync(notification, cancellationToken);
                notificationsAdded = true;
            }
        }

        if (notificationsAdded)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
