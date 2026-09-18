using zogo.Application.DTOs.Properties.PropertyAmenity;
using zogo.Application.DTOs.Property.PropertyAmenity;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyAmenityService
{
    Task<PropertyAmenityResponse> AddAsync(
        Guid userId,
        Guid propertyId,
        AddPropertyAmenityRequest request,
        CancellationToken cancellationToken = default);

    Task<List<PropertyAmenityResponse>> GetByPropertyIdAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid userId,
        Guid propertyId,
        int amenityId,
        CancellationToken cancellationToken = default);
}