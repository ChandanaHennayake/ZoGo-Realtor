using zogo.Domain.Entities.Property;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task AddAsync(
        Property property,
        CancellationToken cancellationToken = default);

    Task<Property?> GetByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);
}