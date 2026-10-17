using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;

namespace zogo.Domain.Entities.Buyer;

public class PropertyOffer
{
    private PropertyOffer()
    {
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid BuyerId { get; private set; }

    public decimal OfferAmount { get; private set; }
    public decimal? CounterAmount { get; private set; }
    public string? BuyerNotes { get; private set; }
    public string? SellerNotes { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    // 1: Submitted, 2: Countered, 3: Accepted, 4: Rejected, 5: Withdrawn, 6: Expired
    public short Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public Domain.Entities.Master.Property? Property { get; private set; }
    public User? Buyer { get; private set; }

    public static PropertyOffer Create(
        Guid propertyId,
        Guid buyerId,
        decimal offerAmount,
        string? buyerNotes,
        DateTime? expiresAt)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        if (offerAmount <= 0)
            throw new ArgumentException("Offer amount must be greater than zero.", nameof(offerAmount));

        return new PropertyOffer
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            BuyerId = buyerId,
            OfferAmount = offerAmount,
            BuyerNotes = buyerNotes,
            ExpiresAt = expiresAt.HasValue
                ? (expiresAt.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(expiresAt.Value, DateTimeKind.Utc)
                    : expiresAt.Value.ToUniversalTime())
                : null,
            Status = 1, // Submitted
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Counter(decimal counterAmount, string? sellerNotes)
    {
        if (Status != 1 && Status != 2)
            throw new InvalidOperationException($"Cannot counter an offer with status {Status}.");

        if (counterAmount <= 0)
            throw new ArgumentException("Counter amount must be greater than zero.", nameof(counterAmount));

        CounterAmount = counterAmount;
        SellerNotes = sellerNotes;
        Status = 2; // Countered
        UpdatedAt = DateTime.UtcNow;
    }

    public void Accept(string? sellerNotes)
    {
        if (Status != 1 && Status != 2)
            throw new InvalidOperationException($"Cannot accept an offer with status {Status}.");

        if (!string.IsNullOrWhiteSpace(sellerNotes))
            SellerNotes = sellerNotes;

        Status = 3; // Accepted
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string? reason)
    {
        if (Status != 1 && Status != 2)
            throw new InvalidOperationException($"Cannot reject an offer with status {Status}.");

        if (!string.IsNullOrWhiteSpace(reason))
            SellerNotes = reason;

        Status = 4; // Rejected
        UpdatedAt = DateTime.UtcNow;
    }

    public void Withdraw(string? reason)
    {
        if (Status != 1 && Status != 2)
            throw new InvalidOperationException($"Cannot withdraw an offer with status {Status}.");

        if (!string.IsNullOrWhiteSpace(reason))
            BuyerNotes = string.IsNullOrWhiteSpace(BuyerNotes) ? reason : $"{BuyerNotes} (Withdrawal reason: {reason})";

        Status = 5; // Withdrawn
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkExpired()
    {
        if (Status == 1 || Status == 2)
        {
            Status = 6; // Expired
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
    }
}
