using zogo.Application.DTOs.Property.PropertyMedia;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Property;

namespace zogo.Application.Services;

public class PropertyMediaService : IPropertyMediaService
{
    private readonly IPropertyMediaRepository _propertyMediaRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyMediaService(
        IPropertyMediaRepository propertyMediaRepository,
        IPropertyRepository propertyRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _propertyMediaRepository = propertyMediaRepository;
        _propertyRepository = propertyRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PropertyMediaResponse> AddAsync(
        Guid userId,
        Guid propertyId,
        CreatePropertyMediaRequest request,
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

        if (request.MediaType <= 0)
            throw new ArgumentException("Media type is required.");

        if (string.IsNullOrWhiteSpace(request.StorageKey))
            throw new ArgumentException("Storage key is required.");

        if (string.IsNullOrWhiteSpace(request.OriginalFileName))
            throw new ArgumentException("Original file name is required.");

        if (string.IsNullOrWhiteSpace(request.MimeType))
            throw new ArgumentException("MIME type is required.");

        if (request.FileSizeBytes <= 0)
            throw new ArgumentException(
                "File size must be greater than zero.");

        if (request.DisplayOrder < 0)
            throw new ArgumentException(
                "Display order cannot be negative.");

        var propertyMedia = PropertyMedia.Create(
            propertyId,
            request.MediaType,
            request.StorageKey,
            request.OriginalFileName,
            request.MimeType,
            request.FileSizeBytes,
            request.DisplayOrder,
            request.IsCover,
            userId);

        await _propertyMediaRepository.AddAsync(
            propertyMedia,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(propertyMedia);
    }

    public async Task<List<PropertyMediaResponse>> GetByPropertyIdAsync(
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

        var media = await _propertyMediaRepository
            .GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        return media
            .Select(MapToResponse)
            .ToList();
    }

    public async Task DeleteAsync(
        Guid userId,
        Guid propertyId,
        Guid mediaId,
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

        var propertyMedia =
            await _propertyMediaRepository.GetByIdAsync(
                mediaId,
                cancellationToken);

        if (propertyMedia is null ||
            propertyMedia.PropertyId != propertyId)
        {
            throw new KeyNotFoundException(
                "Media not found.");
        }

        await _fileStorageService.DeleteAsync(
            propertyMedia.StorageKey,
            cancellationToken);

        await _propertyMediaRepository.DeleteAsync(
            propertyMedia,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    private static PropertyMediaResponse MapToResponse(
        PropertyMedia media)
    {
        return new PropertyMediaResponse
        {
            Id = media.Id,
            PropertyId = media.PropertyId,
            MediaType = media.MediaType,
            StorageKey = media.StorageKey,
            OriginalFileName = media.OriginalFileName,
            MimeType = media.MimeType,
            FileSizeBytes = media.FileSizeBytes,
            DisplayOrder = media.DisplayOrder,
            IsCover = media.IsCover,
            CreatedBy = media.CreatedBy,
            CreatedAt = media.CreatedAt
        };
    }



    public async Task<(Stream Stream, string MimeType, string FileName)> DownloadAsync(
    Guid userId,
    Guid propertyId,
    Guid mediaId,
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

        var media = await _propertyMediaRepository.GetByIdAsync(
            mediaId,
            cancellationToken);

        if (media is null || media.PropertyId != propertyId)
            throw new KeyNotFoundException("Media not found.");

        var stream = await _fileStorageService.DownloadAsync(
            media.StorageKey,
            cancellationToken);

        return (
            stream,
            media.MimeType,
            media.OriginalFileName
        );
    }
}