using zogo.Domain.Entities.Property;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyFinancialsRepository
{
    Task AddAsync(
        PropertyFinancials propertyFinancials,
        CancellationToken cancellationToken = default);

    Task<PropertyFinancials?> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);
}