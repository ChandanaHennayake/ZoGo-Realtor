namespace zogo.Application.DTOs.Seller;

public sealed class SellerActivationResponse
{
    public Guid UserId { get; init; }

    public bool SellerActivated { get; init; }

    public string Message { get; init; } = null!;
}