namespace zogo.Application.DTOs.Property.LegalDetails;

public sealed class GetPropertyLegalDetailsResponse
{
    public Guid PropertyId { get; init; }
    public short? OwnershipType { get; init; }
    public bool HasMortgage { get; init; }
    public string? MortgageProvider { get; init; }
    public bool HasLegalIssues { get; init; }
    public string? LegalIssueDescription { get; init; }
    public bool LegalVerified { get; init; }
    public Guid? VerifiedBy { get; init; }
    public DateTime? VerifiedAt { get; init; }
}