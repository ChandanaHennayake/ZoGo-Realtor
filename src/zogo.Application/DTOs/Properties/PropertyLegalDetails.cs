namespace zogo.Domain.Entities.Property;

public class PropertyLegalDetails
{
    private PropertyLegalDetails()
    {
    }

    public Guid PropertyId { get; private set; }
    public short? OwnershipType { get; private set; }
    public bool HasMortgage { get; private set; }
    public string? MortgageProvider { get; private set; }
    public bool HasLegalIssues { get; private set; }
    public string? LegalIssueDescription { get; private set; }
    public bool LegalVerified { get; private set; }
    public Guid? VerifiedBy { get; private set; }
    public DateTime? VerifiedAt { get; private set; }

    public static PropertyLegalDetails Create(
        Guid propertyId,
        short? ownershipType,
        bool hasMortgage,
        string? mortgageProvider,
        bool hasLegalIssues,
        string? legalIssueDescription,
        bool legalVerified,
        Guid? verifiedBy,
        DateTime? verifiedAt)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        if (!hasMortgage)
        {
            mortgageProvider = null;
        }

        if (!hasLegalIssues)
        {
            legalIssueDescription = null;
        }

        if (!legalVerified)
        {
            verifiedBy = null;
            verifiedAt = null;
        }

        return new PropertyLegalDetails
        {
            PropertyId = propertyId,
            OwnershipType = ownershipType,
            HasMortgage = hasMortgage,
            MortgageProvider = string.IsNullOrWhiteSpace(mortgageProvider)
                ? null
                : mortgageProvider.Trim(),
            HasLegalIssues = hasLegalIssues,
            LegalIssueDescription = string.IsNullOrWhiteSpace(legalIssueDescription)
                ? null
                : legalIssueDescription.Trim(),
            LegalVerified = legalVerified,
            VerifiedBy = verifiedBy,
            VerifiedAt = verifiedAt
        };
    }

    public void Update(
        short? ownershipType,
        bool hasMortgage,
        string? mortgageProvider,
        bool hasLegalIssues,
        string? legalIssueDescription,
        bool legalVerified,
        Guid? verifiedBy,
        DateTime? verifiedAt)
    {
        if (!hasMortgage)
        {
            mortgageProvider = null;
        }

        if (!hasLegalIssues)
        {
            legalIssueDescription = null;
        }

        if (!legalVerified)
        {
            verifiedBy = null;
            verifiedAt = null;
        }

        OwnershipType = ownershipType;
        HasMortgage = hasMortgage;
        MortgageProvider = string.IsNullOrWhiteSpace(mortgageProvider)
            ? null
            : mortgageProvider.Trim();
        HasLegalIssues = hasLegalIssues;
        LegalIssueDescription = string.IsNullOrWhiteSpace(legalIssueDescription)
            ? null
            : legalIssueDescription.Trim();
        LegalVerified = legalVerified;
        VerifiedBy = verifiedBy;
        VerifiedAt = verifiedAt;
    }
}