using zogo.Application.DTOs.Buyer;

namespace zogo.Application.Interfaces.Services;

public interface IBuyerPropertyService
{
    Task<IReadOnlyList<BuyerPropertyResponse>> GetPropertiesAsync(
        BuyerPropertyListQuery query,
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task<BuyerPropertyDetailsResponse?> GetPropertyDetailsAsync(
        Guid propertyId,
        Guid? userId,
        CancellationToken cancellationToken = default);
}
