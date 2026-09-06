using zogo.Application.DTOs.Seller;

namespace zogo.Application.Interfaces.Services;

public interface ISellerService
{
    Task<SellerActivationResponse> ActivateSellerAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}