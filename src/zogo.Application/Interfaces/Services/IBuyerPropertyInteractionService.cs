using zogo.Application.DTOs.Buyer;

namespace zogo.Application.Interfaces.Services;

public interface IBuyerPropertyInteractionService
{
    Task<BuyerPropertyInteractionResponse> SetInteractionStatusAsync(Guid buyerId, Guid propertyId, short currentStatus, CancellationToken cancellationToken = default);
    Task<BuyerPropertyInteractionResponse?> GetInteractionAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default);
}
