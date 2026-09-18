using zogo.Domain.Entities.Property;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyFeatureRepository
{
    Task AddAsync(
        PropertyFeature propertyFeature,
        CancellationToken cancellationToken = default);

    Task<List<PropertyFeature>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<PropertyFeature?> GetAsync(
        Guid propertyId,
        int featureId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid propertyId,
        int featureId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        PropertyFeature propertyFeature,
        CancellationToken cancellationToken = default);
}