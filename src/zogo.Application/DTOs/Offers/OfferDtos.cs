namespace zogo.Application.DTOs.Offers;

public sealed class CreateOfferRequest
{
    public Guid PropertyId { get; set; }
    public decimal OfferAmount { get; set; }
    public string? BuyerNotes { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public sealed class CounterOfferRequest
{
    public decimal CounterAmount { get; set; }
    public string? SellerNotes { get; set; }
}

public sealed class AcceptOfferRequest
{
    public string? SellerNotes { get; set; }
}

public sealed class RejectOfferRequest
{
    public string? Reason { get; set; }
}

public sealed class WithdrawOfferRequest
{
    public string? Reason { get; set; }
}

public sealed class OfferResponse
{
    public Guid OfferId { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = null!;
    public string PropertyReferenceNo { get; set; } = null!;
    public decimal AskingPrice { get; set; }
    public string DistrictName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;

    public Guid BuyerId { get; set; }
    public string BuyerName { get; set; } = null!;
    public string BuyerEmail { get; set; } = string.Empty;
    public string BuyerPhone { get; set; } = string.Empty;

    public Guid SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;

    public decimal OfferAmount { get; set; }
    public decimal? CounterAmount { get; set; }
    public string? BuyerNotes { get; set; }
    public string? SellerNotes { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
