using zogo.Application.DTOs.Properties;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Property;

namespace zogo.Application.Services;

public sealed class PropertyService : IPropertyService
{
    private const short DraftStatus = 1;

    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyService(
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePropertyResponse> CreateDraftAsync(
        Guid userId,
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));
        }

        // =========================================================
        // Validation
        // =========================================================

        if (string.IsNullOrWhiteSpace(request.ReferenceNo))
        {
            throw new ArgumentException(
                "Reference number is required.",
                nameof(request.ReferenceNo));
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "Property title is required.",
                nameof(request.Title));
        }

        if (request.PropertyTypeId <= 0)
        {
            throw new ArgumentException(
                "Property type is required.",
                nameof(request.PropertyTypeId));
        }

        if (request.ListingTypeId <= 0)
        {
            throw new ArgumentException(
                "Listing type is required.",
                nameof(request.ListingTypeId));
        }

        if (request.DistrictId <= 0)
        {
            throw new ArgumentException(
                "District is required.",
                nameof(request.DistrictId));
        }

        if (string.IsNullOrWhiteSpace(request.AddressLine1))
        {
            throw new ArgumentException(
                "Address line 1 is required.",
                nameof(request.AddressLine1));
        }

        if (string.IsNullOrWhiteSpace(request.City))
        {
            throw new ArgumentException(
                "City is required.",
                nameof(request.City));
        }

        if (request.AskingPrice < 0)
        {
            throw new ArgumentException(
                "Asking price cannot be negative.",
                nameof(request.AskingPrice));
        }

        // =========================================================
        // Create property
        // =========================================================

        var property = Property.CreateDraft(
            request.ReferenceNo,
            userId,
            request.PropertyTypeId,
            request.ListingTypeId,
            request.Title,
            request.Description,
            request.DistrictId,
            request.DivisionalSecretariatId,
            request.GnDivisionId,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.PostalCode,
            request.Latitude,
            request.Longitude,
            request.AskingPrice,
            request.IsNegotiable,
            userId);

        // =========================================================
        // Save
        // =========================================================

        await _propertyRepository.AddAsync(
            property,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        // =========================================================
        // Response
        // =========================================================

        return new CreatePropertyResponse
        {
            PropertyId = property.Id,
            ReferenceNo = property.ReferenceNo,
            OwnerUserId = property.OwnerUserId,
            Status = property.Status,
            CreatedAt = property.CreatedAt
        };
    }
}