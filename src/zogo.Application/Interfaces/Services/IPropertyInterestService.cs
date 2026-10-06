using zogo.Application.DTOs.Buyer;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyInterestService
{
    Task<PropertyInterestResponse> SetInterestAsync(Guid buyerId, Guid propertyId, short status, CancellationToken cancellationToken = default);
    Task<PropertyInterestResponse?> GetInterestAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default);
}
