namespace zogo.Application.DTOs.Buyer;

public sealed class RejectInquiryRequest
{
    public Guid? BuyerId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public sealed class MarkSoldRequest
{
    public Guid? BuyerId { get; set; }
}

public sealed class CancelInquiryRequest
{
    public Guid? BuyerId { get; set; }
    public string? Reason { get; set; }
}
