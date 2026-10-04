using zogo.Domain.Entities.Property;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyDocumentRepository
{
    Task AddAsync(
        PropertyDocument propertyDocument,
        CancellationToken cancellationToken = default);

    Task<PropertyDocument?> GetByIdAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<List<PropertyDocument>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        PropertyDocument propertyDocument,
        CancellationToken cancellationToken = default);
}
