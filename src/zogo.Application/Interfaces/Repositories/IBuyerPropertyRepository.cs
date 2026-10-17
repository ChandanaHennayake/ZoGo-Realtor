using zogo.Application.DTOs.Buyer;

namespace zogo.Application.Interfaces.Repositories;

public interface IBuyerPropertyRepository
{
    Task<IReadOnlyList<BuyerPropertyResponse>> GetPublishedPropertiesAsync(
        BuyerPropertyListQuery query,
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task<BuyerPropertyDetailsResponse?> GetPublishedPropertyDetailsAsync(
        Guid propertyId,
        Guid? userId,
        CancellationToken cancellationToken = default);
}
