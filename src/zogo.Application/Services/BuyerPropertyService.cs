using zogo.Application.DTOs.Buyer;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;

namespace zogo.Application.Services;

public sealed class BuyerPropertyService : IBuyerPropertyService
{
    private readonly IBuyerPropertyRepository _buyerPropertyRepository;

    public BuyerPropertyService(IBuyerPropertyRepository buyerPropertyRepository)
    {
        _buyerPropertyRepository = buyerPropertyRepository;
    }

    public async Task<IReadOnlyList<BuyerPropertyResponse>> GetPropertiesAsync(
        BuyerPropertyListQuery query,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        return await _buyerPropertyRepository.GetPublishedPropertiesAsync(
            query,
            userId,
            cancellationToken);
    }

    public async Task<BuyerPropertyDetailsResponse?> GetPropertyDetailsAsync(
        Guid propertyId,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        return await _buyerPropertyRepository.GetPublishedPropertyDetailsAsync(
            propertyId,
            userId,
            cancellationToken);
    }
}
