using zogo.Domain.Entities.Property;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyLegalDetailsRepository
{
    Task AddAsync(
        PropertyLegalDetails legalDetails,
        CancellationToken cancellationToken = default);

    Task<PropertyLegalDetails?> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

}