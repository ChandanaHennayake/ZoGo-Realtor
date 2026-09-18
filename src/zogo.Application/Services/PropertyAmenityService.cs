using zogo.Application.DTOs.Properties.PropertyAmenity;
using zogo.Application.DTOs.Property.PropertyAmenity;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Property;

namespace zogo.Application.Services;

public class PropertyAmenityService : IPropertyAmenityService
{
    private readonly IPropertyAmenityRepository _propertyAmenityRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyAmenityService(
        IPropertyAmenityRepository propertyAmenityRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyAmenityRepository = propertyAmenityRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PropertyAmenityResponse> AddAsync(
        Guid userId,
        Guid propertyId,
        AddPropertyAmenityRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            throw new KeyNotFoundException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You are not authorized to modify this property.");

        if (request.AmenityId <= 0)
            throw new ArgumentException("Amenity ID is required.");

        var exists = await _propertyAmenityRepository.ExistsAsync(
            propertyId,
            request.AmenityId,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "This amenity is already assigned to the property.");

        var propertyAmenity = PropertyAmenity.Create(
            propertyId,
            request.AmenityId);

        await _propertyAmenityRepository.AddAsync(
            propertyAmenity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PropertyAmenityResponse
        {
            PropertyId = propertyId,
            AmenityId = request.AmenityId
        };
    }

    public async Task<List<PropertyAmenityResponse>> GetByPropertyIdAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            throw new KeyNotFoundException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You are not authorized to access this property.");

        var amenities =
            await _propertyAmenityRepository.GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        return amenities
            .Select(x => new PropertyAmenityResponse
            {
                PropertyId = x.PropertyId,
                AmenityId = x.AmenityId,
                AmenityCode = x.Amenity.Code,
                AmenityName = x.Amenity.Name,
                Category = x.Amenity.Category,
                DisplayOrder = x.Amenity.DisplayOrder
            })
            .ToList();
    }

    public async Task DeleteAsync(
        Guid userId,
        Guid propertyId,
        int amenityId,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            throw new KeyNotFoundException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You are not authorized to modify this property.");

        var propertyAmenity =
            await _propertyAmenityRepository.GetAsync(
                propertyId,
                amenityId,
                cancellationToken);

        if (propertyAmenity is null)
            throw new KeyNotFoundException(
                "Amenity is not assigned to this property.");

        await _propertyAmenityRepository.DeleteAsync(
            propertyAmenity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}