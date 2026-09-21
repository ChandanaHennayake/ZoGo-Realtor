using zogo.Application.DTOs.Property.PropertyMedia;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyMediaService
{
    Task<PropertyMediaResponse> AddAsync(
        Guid userId,
        Guid propertyId,
        CreatePropertyMediaRequest request,
        CancellationToken cancellationToken = default);

    Task<List<PropertyMediaResponse>> GetByPropertyIdAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid userId,
        Guid propertyId,
        Guid mediaId,
        CancellationToken cancellationToken = default);


    Task<(Stream Stream, string MimeType, string FileName)> DownloadAsync(
    Guid userId,
    Guid propertyId,
    Guid mediaId,
    CancellationToken cancellationToken = default);


}