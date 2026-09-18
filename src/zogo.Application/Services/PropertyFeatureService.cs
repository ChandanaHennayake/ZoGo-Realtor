using zogo.Application.DTOs.Property.PropertyFeature;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Property;

namespace zogo.Application.Services;

public class PropertyFeatureService : IPropertyFeatureService
{
    private readonly IPropertyFeatureRepository _propertyFeatureRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyFeatureService(
        IPropertyFeatureRepository propertyFeatureRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyFeatureRepository = propertyFeatureRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PropertyFeatureResponse> AddAsync(
        Guid userId,
        Guid propertyId,
        AddPropertyFeatureRequest request,
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

        if (request.FeatureId <= 0)
            throw new ArgumentException("Feature ID is required.");

        var exists = await _propertyFeatureRepository.ExistsAsync(
            propertyId,
            request.FeatureId,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "This feature is already assigned to the property.");

        var propertyFeature = PropertyFeature.Create(
            propertyId,
            request.FeatureId);

        await _propertyFeatureRepository.AddAsync(
            propertyFeature,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PropertyFeatureResponse
        {
            PropertyId = propertyId,
            FeatureId = request.FeatureId
        };
    }

    public async Task<List<PropertyFeatureResponse>> GetByPropertyIdAsync(
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

        var features = await _propertyFeatureRepository.GetByPropertyIdAsync(
            propertyId,
            cancellationToken);

        return features
          .Select(x => new PropertyFeatureResponse
          {
              PropertyId = x.PropertyId,
              FeatureId = x.FeatureId,
              FeatureCode = x.Feature.Code,
              FeatureName = x.Feature.Name,
              Category = x.Feature.Category,
              DisplayOrder = x.Feature.DisplayOrder
          })
          .ToList();
    }

    public async Task DeleteAsync(
        Guid userId,
        Guid propertyId,
        int featureId,
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

        var propertyFeature = await _propertyFeatureRepository.GetAsync(
            propertyId,
            featureId,
            cancellationToken);

        if (propertyFeature is null)
            throw new KeyNotFoundException(
                "Feature is not assigned to this property.");

        await _propertyFeatureRepository.DeleteAsync(
            propertyFeature,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}