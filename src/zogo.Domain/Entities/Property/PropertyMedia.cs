namespace zogo.Domain.Entities.Property;

public class PropertyMedia
{
    private PropertyMedia()
    {
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public short MediaType { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public string OriginalFileName { get; private set; } = null!;
    public string MimeType { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsCover { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static PropertyMedia Create(
        Guid propertyId,
        short mediaType,
        string storageKey,
        string originalFileName,
        string mimeType,
        long fileSizeBytes,
        int displayOrder,
        bool isCover,
        Guid createdBy)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        if (mediaType <= 0)
            throw new ArgumentException(
                "Media type is required.",
                nameof(mediaType));

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException(
                "Storage key is required.",
                nameof(storageKey));

        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new ArgumentException(
                "Original file name is required.",
                nameof(originalFileName));

        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException(
                "MIME type is required.",
                nameof(mimeType));

        if (fileSizeBytes <= 0)
            throw new ArgumentException(
                "File size must be greater than zero.",
                nameof(fileSizeBytes));

        if (displayOrder < 0)
            throw new ArgumentException(
                "Display order cannot be negative.",
                nameof(displayOrder));

        if (createdBy == Guid.Empty)
            throw new ArgumentException(
                "Created by user is required.",
                nameof(createdBy));

        return new PropertyMedia
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            MediaType = mediaType,
            StorageKey = storageKey,
            OriginalFileName = originalFileName,
            MimeType = mimeType,
            FileSizeBytes = fileSizeBytes,
            DisplayOrder = displayOrder,
            IsCover = isCover,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
}