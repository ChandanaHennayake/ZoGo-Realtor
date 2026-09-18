using zogo.Application.DTOs.Property.PropertyFeature;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyFeatureService
{
    Task<PropertyFeatureResponse> AddAsync(
        Guid userId,
        Guid propertyId,
        AddPropertyFeatureRequest request,
        CancellationToken cancellationToken = default);

    Task<List<PropertyFeatureResponse>> GetByPropertyIdAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid userId,
        Guid propertyId,
        int featureId,
        CancellationToken cancellationToken = default);
}