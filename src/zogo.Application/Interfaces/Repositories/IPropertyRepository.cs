using zogo.Domain.Entities.Master;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task AddAsync(
        Property property,
        CancellationToken cancellationToken = default);

    Task<Property?> GetByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> GetByOwnerUserIdAsync(
    Guid ownerUserId,
    CancellationToken cancellationToken = default);


}