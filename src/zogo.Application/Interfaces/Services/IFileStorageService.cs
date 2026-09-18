namespace zogo.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> UploadAsync(
        Stream stream,
        string storageKey,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default);
}