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




    public async Task<GetPropertyResponse?> GetByIdAsync(
    Guid propertyId,
    CancellationToken cancellationToken = default)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null)
            return null;

        return new GetPropertyResponse
        {
            PropertyId = property.Id,
            ReferenceNo = property.ReferenceNo,

            OwnerUserId = property.OwnerUserId,

            PropertyTypeId = property.PropertyTypeId,
            ListingTypeId = property.ListingTypeId,

            Title = property.Title,
            Description = property.Description,

            DistrictId = property.DistrictId,
            DivisionalSecretariatId = property.DivisionalSecretariatId,
            GnDivisionId = property.GnDivisionId,

            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            City = property.City,
            PostalCode = property.PostalCode,

            Latitude = property.Latitude,
            Longitude = property.Longitude,

            AskingPrice = property.AskingPrice,
            IsNegotiable = property.IsNegotiable,

            Status = property.Status,

            PublishedAt = property.PublishedAt,
            SoldAt = property.SoldAt,

            CreatedBy = property.CreatedBy,
            CreatedAt = property.CreatedAt,

            UpdatedBy = property.UpdatedBy,
            UpdatedAt = property.UpdatedAt
        };
    }

    public async Task<IReadOnlyList<GetPropertyResponse>> GetMyPropertiesAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));

        var properties = await _propertyRepository.GetByOwnerUserIdAsync(
            userId,
            cancellationToken);

        return properties
            .Select(property => new GetPropertyResponse
            {
                PropertyId = property.Id,
                ReferenceNo = property.ReferenceNo,
                OwnerUserId = property.OwnerUserId,

                PropertyTypeId = property.PropertyTypeId,
                ListingTypeId = property.ListingTypeId,

                Title = property.Title,
                Description = property.Description,

                DistrictId = property.DistrictId,
                DivisionalSecretariatId = property.DivisionalSecretariatId,
                GnDivisionId = property.GnDivisionId,

                AddressLine1 = property.AddressLine1,
                AddressLine2 = property.AddressLine2,
                City = property.City,
                PostalCode = property.PostalCode,

                Latitude = property.Latitude,
                Longitude = property.Longitude,

                AskingPrice = property.AskingPrice,
                IsNegotiable = property.IsNegotiable,

                Status = property.Status,

                PublishedAt = property.PublishedAt,
                SoldAt = property.SoldAt,

                CreatedBy = property.CreatedBy,
                CreatedAt = property.CreatedAt,

                UpdatedBy = property.UpdatedBy,
                UpdatedAt = property.UpdatedAt
            })
            .ToList();
    }


    public async Task<bool> UpdateAsync(
     Guid userId,
     Guid propertyId,
     UpdatePropertyRequest request,
     CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        if (request.PropertyTypeId <= 0)
            throw new ArgumentException(
                "Property type is required.",
                nameof(request.PropertyTypeId));

        if (request.ListingTypeId <= 0)
            throw new ArgumentException(
                "Listing type is required.",
                nameof(request.ListingTypeId));

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException(
                "Property title is required.",
                nameof(request.Title));

        if (request.DistrictId <= 0)
            throw new ArgumentException(
                "District is required.",
                nameof(request.DistrictId));

        if (string.IsNullOrWhiteSpace(request.AddressLine1))
            throw new ArgumentException(
                "Address line 1 is required.",
                nameof(request.AddressLine1));

        if (string.IsNullOrWhiteSpace(request.City))
            throw new ArgumentException(
                "City is required.",
                nameof(request.City));

        if (request.AskingPrice < 0)
            throw new ArgumentException(
                "Asking price cannot be negative.",
                nameof(request.AskingPrice));

        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null)
            return false;

        if (property.OwnerUserId != userId)
            return false;

        if (property.Status != 1)
            throw new InvalidOperationException(
                "Only draft properties can be updated.");

        property.Update(
            request.Title,
            request.Description,
            request.PropertyTypeId,
            request.ListingTypeId,
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}