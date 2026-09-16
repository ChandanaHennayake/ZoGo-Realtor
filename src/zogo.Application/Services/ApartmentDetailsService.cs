using zogo.Application.DTOs.Properties;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Property;

namespace zogo.Application.Services;

public sealed class ApartmentDetailsService
    : IApartmentDetailsService
{
    private readonly IApartmentDetailsRepository _apartmentDetailsRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApartmentDetailsService(
        IApartmentDetailsRepository apartmentDetailsRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _apartmentDetailsRepository = apartmentDetailsRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetApartmentDetailsResponse> CreateAsync(
        Guid userId,
        Guid propertyId,
        CreateApartmentDetailsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        ArgumentNullException.ThrowIfNull(request);

        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null)
            throw new KeyNotFoundException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You do not own this property.");

        var exists = await _apartmentDetailsRepository.ExistsAsync(
            propertyId,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "Apartment details already exist for this property.");

        var apartmentDetails = ApartmentDetails.Create(
            propertyId,
            request.CondominiumId,
            request.ApartmentTypeId,
            request.FloorNumber,
            request.UnitNumber,
            request.IsCornerUnit,
            request.FloorAreaSqFt,
            request.Bedrooms,
            request.MasterBedrooms,
            request.Bathrooms,
            request.AttachedBathrooms,
            request.Balconies,
            request.FurnishingTypeId,
            request.ViewTypeId,
            request.HasMaidRoom,
            request.HasMaidBathroom,
            request.HasLaundryArea,
            request.HasStorageRoom,
            request.HasWalkInCloset,
            request.HasDriversRoom);

        await _apartmentDetailsRepository.AddAsync(
            apartmentDetails,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(apartmentDetails);
    }

    public async Task<GetApartmentDetailsResponse?> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        var apartmentDetails =
            await _apartmentDetailsRepository.GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        return apartmentDetails is null
            ? null
            : Map(apartmentDetails);
    }

    public async Task<bool> UpdateAsync(
        Guid userId,
        Guid propertyId,
        UpdateApartmentDetailsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        ArgumentNullException.ThrowIfNull(request);

        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null)
            return false;

        if (property.OwnerUserId != userId)
            return false;

        var apartmentDetails =
            await _apartmentDetailsRepository.GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        if (apartmentDetails is null)
            return false;

        apartmentDetails.Update(
            request.CondominiumId,
            request.ApartmentTypeId,
            request.FloorNumber,
            request.UnitNumber,
            request.IsCornerUnit,
            request.FloorAreaSqFt,
            request.Bedrooms,
            request.MasterBedrooms,
            request.Bathrooms,
            request.AttachedBathrooms,
            request.Balconies,
            request.FurnishingTypeId,
            request.ViewTypeId,
            request.HasMaidRoom,
            request.HasMaidBathroom,
            request.HasLaundryArea,
            request.HasStorageRoom,
            request.HasWalkInCloset,
            request.HasDriversRoom);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static GetApartmentDetailsResponse Map(
        ApartmentDetails apartmentDetails)
    {
        return new GetApartmentDetailsResponse
        {
            PropertyId = apartmentDetails.PropertyId,
            CondominiumId = apartmentDetails.CondominiumId,
            ApartmentTypeId = apartmentDetails.ApartmentTypeId,
            FloorNumber = apartmentDetails.FloorNumber,
            UnitNumber = apartmentDetails.UnitNumber,
            IsCornerUnit = apartmentDetails.IsCornerUnit,
            FloorAreaSqFt = apartmentDetails.FloorAreaSqFt,
            Bedrooms = apartmentDetails.Bedrooms,
            MasterBedrooms = apartmentDetails.MasterBedrooms,
            Bathrooms = apartmentDetails.Bathrooms,
            AttachedBathrooms = apartmentDetails.AttachedBathrooms,
            Balconies = apartmentDetails.Balconies,
            FurnishingTypeId = apartmentDetails.FurnishingTypeId,
            ViewTypeId = apartmentDetails.ViewTypeId,
            HasMaidRoom = apartmentDetails.HasMaidRoom,
            HasMaidBathroom = apartmentDetails.HasMaidBathroom,
            HasLaundryArea = apartmentDetails.HasLaundryArea,
            HasStorageRoom = apartmentDetails.HasStorageRoom,
            HasWalkInCloset = apartmentDetails.HasWalkInCloset,
            HasDriversRoom = apartmentDetails.HasDriversRoom
        };
    }
}