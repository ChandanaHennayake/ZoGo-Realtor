using zogo.Domain.Entities.Property;

namespace zogo.Application.Interfaces.Repositories;

public interface IApartmentDetailsRepository
{
    Task AddAsync(
        ApartmentDetails apartmentDetails,
        CancellationToken cancellationToken = default);

    Task<ApartmentDetails?> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);
}