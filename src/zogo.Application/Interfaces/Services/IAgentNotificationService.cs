namespace zogo.Application.Interfaces.Services;

public interface IAgentNotificationService
{
    Task NotifyAgentsOnBuyerInterestAsync(Guid propertyId, Guid buyerId, CancellationToken cancellationToken = default);
}
