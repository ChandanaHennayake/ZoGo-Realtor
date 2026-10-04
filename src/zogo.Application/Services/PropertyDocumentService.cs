using zogo.Application.DTOs.Property.PropertyDocument;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Property;
using zogo.Domain.Enums;

namespace zogo.Application.Services;

public class PropertyDocumentService : IPropertyDocumentService
{
    private readonly IPropertyDocumentRepository _propertyDocumentRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyDocumentService(
        IPropertyDocumentRepository propertyDocumentRepository,
        IPropertyRepository propertyRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _propertyDocumentRepository = propertyDocumentRepository;
        _propertyRepository = propertyRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PropertyDocumentResponse> AddAsync(
        Guid userId,
        Guid propertyId,
        CreatePropertyDocumentRequest request,
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

        if (request.DocumentTypeId <= 0)
            throw new ArgumentException("Document type ID is required.");

        if (string.IsNullOrWhiteSpace(request.StorageKey))
            throw new ArgumentException("Storage key is required.");

        var propertyDocument = PropertyDocument.Create(
            propertyId,
            request.DocumentTypeId,
            request.StorageKey,
            request.OriginalFileName,
            request.MimeType,
            request.FileSizeBytes,
            userId,
            (short)PropertyDocumentStatus.Uploaded);

        await _propertyDocumentRepository.AddAsync(
            propertyDocument,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(propertyDocument);
    }

    public async Task<PropertyDocumentResponse> GetByIdAsync(
        Guid userId,
        Guid propertyId,
        Guid documentId,
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

        var document = await _propertyDocumentRepository.GetByIdAsync(
            documentId,
            cancellationToken);

        if (document is null || document.PropertyId != propertyId)
            throw new KeyNotFoundException("Document not found.");

        return MapToResponse(document);
    }

    public async Task<List<PropertyDocumentResponse>> GetByPropertyIdAsync(
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

        var documents = await _propertyDocumentRepository
            .GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        return documents
            .Select(MapToResponse)
            .ToList();
    }

    public async Task DeleteAsync(
        Guid userId,
        Guid propertyId,
        Guid documentId,
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

        var document = await _propertyDocumentRepository.GetByIdAsync(
            documentId,
            cancellationToken);

        if (document is null || document.PropertyId != propertyId)
            throw new KeyNotFoundException("Document not found.");

        await _fileStorageService.DeleteAsync(
            document.StorageKey,
            cancellationToken);

        await _propertyDocumentRepository.DeleteAsync(
            document,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<(Stream Stream, string MimeType, string FileName)> DownloadAsync(
        Guid userId,
        Guid propertyId,
        Guid documentId,
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

        var document = await _propertyDocumentRepository.GetByIdAsync(
            documentId,
            cancellationToken);

        if (document is null || document.PropertyId != propertyId)
            throw new KeyNotFoundException("Document not found.");

        var stream = await _fileStorageService.DownloadAsync(
            document.StorageKey,
            cancellationToken);

        return (
            stream,
            document.MimeType ?? "application/octet-stream",
            document.OriginalFileName ?? "document"
        );
    }

    public async Task<PropertyDocumentResponse> UpdateStatusAsync(
        Guid userId,
        Guid propertyId,
        Guid documentId,
        UpdatePropertyDocumentStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            throw new KeyNotFoundException("Property not found.");

        var document = await _propertyDocumentRepository.GetByIdAsync(
            documentId,
            cancellationToken);

        if (document is null || document.PropertyId != propertyId)
            throw new KeyNotFoundException("Document not found.");

        document.UpdateStatus(request.Status, userId, request.RejectionReason);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(document);
    }

    private static PropertyDocumentResponse MapToResponse(PropertyDocument doc)
    {
        var statusEnum = (PropertyDocumentStatus)doc.Status;
        var statusName = statusEnum switch
        {
            PropertyDocumentStatus.Uploaded => "UPLOADED",
            PropertyDocumentStatus.UnderReview => "UNDER_REVIEW",
            PropertyDocumentStatus.Verified => "VERIFIED",
            PropertyDocumentStatus.Rejected => "REJECTED",
            _ => statusEnum.ToString()
        };

        return new PropertyDocumentResponse
        {
            Id = doc.Id,
            PropertyId = doc.PropertyId,
            DocumentTypeId = doc.DocumentTypeId,
            StorageKey = doc.StorageKey,
            OriginalFileName = doc.OriginalFileName,
            MimeType = doc.MimeType,
            FileSizeBytes = doc.FileSizeBytes,
            Status = doc.Status,
            StatusName = statusName,
            UploadedBy = doc.UploadedBy,
            UploadedAt = doc.UploadedAt,
            VerifiedBy = doc.VerifiedBy,
            VerifiedAt = doc.VerifiedAt,
            RejectionReason = doc.RejectionReason
        };
    }
}
