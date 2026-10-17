namespace zogo.Application.DTOs.Transactions;

public sealed class InitiateTransactionRequest
{
    public Guid PropertyId { get; set; }
    public Guid? OfferId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid? AgentId { get; set; }
    public decimal AgreedPrice { get; set; }
    public decimal CommissionRate { get; set; } = 0.03m; // 3% standard rate
    public string? Notes { get; set; }
}

public sealed class VerifyTitleRequest
{
    public string TitleDeedReference { get; set; } = null!;
    public string NotaryName { get; set; } = null!;
    public string? Notes { get; set; }
}

public sealed class SignDeedRequest
{
    public string? Notes { get; set; }
}

public sealed class SettleTransactionRequest
{
    public string? ClosingNotes { get; set; }
}

public sealed class CancelTransactionRequest
{
    public string Reason { get; set; } = null!;
}

public sealed class TransactionResponse
{
    public Guid TransactionId { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = null!;
    public string PropertyReferenceNo { get; set; } = null!;
    public string DistrictName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;

    public Guid? OfferId { get; set; }

    public Guid BuyerId { get; set; }
    public string BuyerName { get; set; } = null!;
    public string BuyerEmail { get; set; } = string.Empty;
    public string BuyerPhone { get; set; } = string.Empty;

    public Guid SellerId { get; set; }
    public string SellerName { get; set; } = null!;
    public string SellerEmail { get; set; } = string.Empty;
    public string SellerPhone { get; set; } = string.Empty;

    public Guid? AgentId { get; set; }
    public string? AgentName { get; set; }

    public decimal AgreedPrice { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }

    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public string? TitleDeedReference { get; set; }
    public string? NotaryName { get; set; }
    public string? ClosingNotes { get; set; }
    public DateTime? ClosingDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CommissionReportResponse
{
    public decimal TotalSettledVolume { get; set; }
    public decimal TotalCommissionEarned { get; set; }
    public int CompletedDealsCount { get; set; }
    public int ActiveTransactionsCount { get; set; }
    public IReadOnlyList<AgentCommissionItem> AgentBreakdown { get; set; } = Array.Empty<AgentCommissionItem>();
}

public sealed class AgentCommissionItem
{
    public Guid AgentId { get; set; }
    public string AgentName { get; set; } = null!;
    public string Email { get; set; } = string.Empty;
    public int DealsClosed { get; set; }
    public decimal TotalSalesVolume { get; set; }
    public decimal CommissionEarned { get; set; }
}
