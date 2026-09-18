using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using zogo.Application.Interfaces.Services;

namespace zogo.Infrastructure.Storage;

public class CloudflareR2StorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public CloudflareR2StorageService(IConfiguration configuration)
    {
        var accountId = configuration["CloudflareR2:AccountId"]
            ?? throw new InvalidOperationException(
                "Cloudflare R2 AccountId is missing.");

        var accessKey = configuration["CloudflareR2:AccessKey"]
            ?? throw new InvalidOperationException(
                "Cloudflare R2 AccessKey is missing.");

        var secretKey = configuration["CloudflareR2:SecretKey"]
            ?? throw new InvalidOperationException(
                "Cloudflare R2 SecretKey is missing.");

        _bucketName = configuration["CloudflareR2:BucketName"]
            ?? throw new InvalidOperationException(
                "Cloudflare R2 BucketName is missing.");

        var config = new AmazonS3Config
        {
            ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true
        };

        _s3Client = new AmazonS3Client(
            accessKey,
            secretKey,
            config);
    }

    public async Task<string> UploadAsync(
    Stream stream,
    string storageKey,
    string contentType,
    CancellationToken cancellationToken = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException(
                "Storage key is required.",
                nameof(storageKey));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException(
                "Content type is required.",
                nameof(contentType));

        if (!stream.CanRead)
            throw new ArgumentException(
                "Stream must be readable.",
                nameof(stream));

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = storageKey,
            InputStream = stream,
            ContentType = contentType,

            // Important for Cloudflare R2:
            UseChunkEncoding = false
        };

        if (stream.CanSeek)
        {
            request.Headers.ContentLength = stream.Length;
        }

        await _s3Client.PutObjectAsync(
            request,
            cancellationToken);

        return storageKey;
    }

    public async Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException(
                "Storage key is required.",
                nameof(storageKey));

        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = storageKey
        };

        await _s3Client.DeleteObjectAsync(
            request,
            cancellationToken);
    }
}