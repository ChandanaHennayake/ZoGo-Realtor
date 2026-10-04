using zogo.Application.DTOs.Property.PropertyDocument;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyDocumentService
{
    Task<PropertyDocumentResponse> AddAsync(
        Guid userId,
        Guid propertyId,
        CreatePropertyDocumentRequest request,
        CancellationToken cancellationToken = default);

    Task<PropertyDocumentResponse> GetByIdAsync(
        Guid userId,
        Guid propertyId,
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<List<PropertyDocumentResponse>> GetByPropertyIdAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid userId,
        Guid propertyId,
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<(Stream Stream, string MimeType, string FileName)> DownloadAsync(
        Guid userId,
        Guid propertyId,
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<PropertyDocumentResponse> UpdateStatusAsync(
        Guid userId,
        Guid propertyId,
        Guid documentId,
        UpdatePropertyDocumentStatusRequest request,
        CancellationToken cancellationToken = default);
}
