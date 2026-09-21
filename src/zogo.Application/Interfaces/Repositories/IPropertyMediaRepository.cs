using zogo.Domain.Entities.Property;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyMediaRepository
{
    Task AddAsync(
        PropertyMedia propertyMedia,
        CancellationToken cancellationToken = default);

    Task<PropertyMedia?> GetByIdAsync(
        Guid mediaId,
        CancellationToken cancellationToken = default);

    Task<List<PropertyMedia>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid mediaId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        PropertyMedia propertyMedia,
        CancellationToken cancellationToken = default);
}