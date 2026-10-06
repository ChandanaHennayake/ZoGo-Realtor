using zogo.Domain.Entities.Buyer;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyInterestRepository
{
    Task AddAsync(PropertyInterest propertyInterest, CancellationToken cancellationToken = default);
    Task<PropertyInterest?> GetAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default);
}
