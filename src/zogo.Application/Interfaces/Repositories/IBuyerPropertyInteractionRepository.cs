using zogo.Domain.Entities.Buyer;

namespace zogo.Application.Interfaces.Repositories;

public interface IBuyerPropertyInteractionRepository
{
    Task AddAsync(BuyerPropertyInteraction interaction, CancellationToken cancellationToken = default);
    Task<BuyerPropertyInteraction?> GetAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default);
}
