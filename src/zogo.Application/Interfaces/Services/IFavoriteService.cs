using zogo.Application.DTOs.Properties;

namespace zogo.Application.Interfaces.Services;

public interface IFavoriteService
{
    Task<bool> AddFavoriteAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<bool> RemoveFavoriteAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GetPropertyResponse>> GetFavoritesAsync(Guid userId, CancellationToken cancellationToken = default);
}
