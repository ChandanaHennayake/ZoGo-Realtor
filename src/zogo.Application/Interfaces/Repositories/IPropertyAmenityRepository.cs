using zogo.Domain.Entities.Property;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyAmenityRepository
{
    Task AddAsync(
        PropertyAmenity propertyAmenity,
        CancellationToken cancellationToken = default);

    Task<List<PropertyAmenity>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<PropertyAmenity?> GetAsync(
        Guid propertyId,
        int amenityId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid propertyId,
        int amenityId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        PropertyAmenity propertyAmenity,
        CancellationToken cancellationToken = default);
}