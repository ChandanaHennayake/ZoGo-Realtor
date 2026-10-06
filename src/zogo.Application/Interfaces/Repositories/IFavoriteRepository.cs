using zogo.Domain.Entities.Buyer;
using zogo.Domain.Entities.Master;

namespace zogo.Application.Interfaces.Repositories;

public interface IFavoriteRepository
{
    Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default);
    Task DeleteAsync(Favorite favorite, CancellationToken cancellationToken = default);
    Task<Favorite?> GetAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Property>> GetFavoritePropertiesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
