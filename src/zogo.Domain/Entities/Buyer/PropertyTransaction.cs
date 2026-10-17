using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;

namespace zogo.Domain.Entities.Buyer;

public class PropertyTransaction
{
    private PropertyTransaction()
    {
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid? OfferId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid SellerId { get; private set; }
    public Guid? AgentId { get; private set; }

    public decimal AgreedPrice { get; private set; }
    public decimal CommissionRate { get; private set; }
    public decimal CommissionAmount { get; private set; }

    // 1: INITIATED, 2: TITLE_VERIFIED, 3: DEED_SIGNED, 4: SETTLED, 5: CANCELLED
    public short Status { get; private set; }

    public string? TitleDeedReference { get; private set; }
    public string? NotaryName { get; private set; }
    public string? ClosingNotes { get; private set; }
    public DateTime? ClosingDate { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public Domain.Entities.Master.Property? Property { get; private set; }
    public PropertyOffer? Offer { get; private set; }
    public User? Buyer { get; private set; }
    public User? Seller { get; private set; }
    public User? Agent { get; private set; }

    public static PropertyTransaction Initiate(
        Guid propertyId,
        Guid? offerId,
        Guid buyerId,
        Guid sellerId,
        Guid? agentId,
        decimal agreedPrice,
        decimal commissionRate,
        string? notes)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        if (sellerId == Guid.Empty)
            throw new ArgumentException("Seller ID is required.", nameof(sellerId));

        if (agreedPrice <= 0)
            throw new ArgumentException("Agreed price must be greater than zero.", nameof(agreedPrice));

        if (commissionRate < 0 || commissionRate > 1)
            throw new ArgumentException("Commission rate must be between 0.00 and 1.00.", nameof(commissionRate));

        var commAmount = Math.Round(agreedPrice * commissionRate, 2);

        return new PropertyTransaction
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            OfferId = offerId,
            BuyerId = buyerId,
            SellerId = sellerId,
            AgentId = agentId,
            AgreedPrice = agreedPrice,
            CommissionRate = commissionRate,
            CommissionAmount = commAmount,
            Status = 1, // INITIATED
            ClosingNotes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void VerifyTitle(string titleDeedReference, string notaryName, string? notes)
    {
        if (Status != 1)
            throw new InvalidOperationException($"Cannot verify title in transaction state {Status}.");

        TitleDeedReference = titleDeedReference;
        NotaryName = notaryName;
        if (!string.IsNullOrWhiteSpace(notes))
            ClosingNotes = string.IsNullOrWhiteSpace(ClosingNotes) ? notes : $"{ClosingNotes}\n[Title]: {notes}";

        Status = 2; // TITLE_VERIFIED
        UpdatedAt = DateTime.UtcNow;
    }

    public void SignDeed(string? notes)
    {
        if (Status != 2)
            throw new InvalidOperationException($"Deed can only be signed after title verification (current status {Status}).");

        if (!string.IsNullOrWhiteSpace(notes))
            ClosingNotes = string.IsNullOrWhiteSpace(ClosingNotes) ? notes : $"{ClosingNotes}\n[Deed]: {notes}";

        Status = 3; // DEED_SIGNED
        UpdatedAt = DateTime.UtcNow;
    }

    public void Settle(string? closingNotes)
    {
        if (Status != 3)
            throw new InvalidOperationException($"Transaction can only be settled after deed is signed (current status {Status}).");

        if (!string.IsNullOrWhiteSpace(closingNotes))
            ClosingNotes = string.IsNullOrWhiteSpace(ClosingNotes) ? closingNotes : $"{ClosingNotes}\n[Settlement]: {closingNotes}";

        Status = 4; // SETTLED
        ClosingDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == 4)
            throw new InvalidOperationException("Cannot cancel a completed transaction.");

        ClosingNotes = string.IsNullOrWhiteSpace(ClosingNotes) ? $"Cancelled: {reason}" : $"{ClosingNotes}\n[Cancelled]: {reason}";
        Status = 5; // CANCELLED
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
    }
}
