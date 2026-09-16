using zogo.Application.DTOs.Property.LegalDetails;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Property;

namespace zogo.Application.Services;

public sealed class PropertyLegalDetailsService : IPropertyLegalDetailsService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyLegalDetailsRepository _legalDetailsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyLegalDetailsService(
        IPropertyRepository propertyRepository,
        IPropertyLegalDetailsRepository legalDetailsRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _legalDetailsRepository = legalDetailsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetPropertyLegalDetailsResponse> CreateAsync(
        Guid userId,
        Guid propertyId,
        CreatePropertyLegalDetailsRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            throw new KeyNotFoundException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You are not the owner of this property.");

        if (await _legalDetailsRepository.ExistsAsync(
                propertyId,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Legal details already exist for this property.");
        }

        var legalDetails = PropertyLegalDetails.Create(
            propertyId,
            request.OwnershipType,
            request.HasMortgage,
            request.MortgageProvider,
            request.HasLegalIssues,
            request.LegalIssueDescription,
            request.LegalVerified,
            request.VerifiedBy,
            request.VerifiedAt);

        await _legalDetailsRepository.AddAsync(
            legalDetails,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(legalDetails);
    }

    public async Task<GetPropertyLegalDetailsResponse?> GetByPropertyIdAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            return null;

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You are not the owner of this property.");

        var legalDetails = await _legalDetailsRepository
            .GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        return legalDetails is null
            ? null
            : MapToResponse(legalDetails);
    }

    public async Task<GetPropertyLegalDetailsResponse> UpdateAsync(
        Guid userId,
        Guid propertyId,
        UpdatePropertyLegalDetailsRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            throw new KeyNotFoundException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You are not the owner of this property.");

        var legalDetails = await _legalDetailsRepository
            .GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        if (legalDetails is null)
            throw new KeyNotFoundException(
                "Legal details not found for this property.");

        legalDetails.Update(
            request.OwnershipType,
            request.HasMortgage,
            request.MortgageProvider,
            request.HasLegalIssues,
            request.LegalIssueDescription,
            request.LegalVerified,
            request.VerifiedBy,
            request.VerifiedAt);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(legalDetails);
    }

    private static GetPropertyLegalDetailsResponse MapToResponse(
        PropertyLegalDetails legalDetails)
    {
        return new GetPropertyLegalDetailsResponse
        {
            PropertyId = legalDetails.PropertyId,
            OwnershipType = legalDetails.OwnershipType,
            HasMortgage = legalDetails.HasMortgage,
            MortgageProvider = legalDetails.MortgageProvider,
            HasLegalIssues = legalDetails.HasLegalIssues,
            LegalIssueDescription = legalDetails.LegalIssueDescription,
            LegalVerified = legalDetails.LegalVerified,
            VerifiedBy = legalDetails.VerifiedBy,
            VerifiedAt = legalDetails.VerifiedAt
        };
    }
}