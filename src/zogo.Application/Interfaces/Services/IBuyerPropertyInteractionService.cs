using zogo.Application.DTOs.Buyer;

namespace zogo.Application.Interfaces.Services;

public interface IBuyerPropertyInteractionService
{
    Task<BuyerPropertyInteractionResponse> SetInteractionStatusAsync(
        Guid buyerId,
        Guid propertyId,
        short currentStatus,
        CancellationToken cancellationToken = default);

    Task<BuyerPropertyInteractionResponse> RequestDocumentsAsync(
        Guid buyerId,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<BuyerPropertyInteractionResponse?> GetInteractionAsync(
        Guid buyerId,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<BuyerPropertyInteractionResponse> MarkSoldAsync(
        Guid userId,
        Guid propertyId,
        Guid? buyerId = null,
        CancellationToken cancellationToken = default);

    Task<BuyerPropertyInteractionResponse> RejectInquiryAsync(
        Guid userId,
        Guid propertyId,
        Guid? buyerId,
        string reason,
        CancellationToken cancellationToken = default);

    Task<BuyerPropertyInteractionResponse> CancelInquiryAsync(
        Guid userId,
        Guid propertyId,
        Guid? buyerId,
        string? reason = null,
        CancellationToken cancellationToken = default);
}
