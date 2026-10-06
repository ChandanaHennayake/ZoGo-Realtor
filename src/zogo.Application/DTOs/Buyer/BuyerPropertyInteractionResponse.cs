namespace zogo.Application.DTOs.Buyer;

public sealed class BuyerPropertyInteractionResponse
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public Guid BuyerId { get; set; }
    public short CurrentStatus { get; set; }
    public string CurrentStatusName { get; set; } = null!;
}
