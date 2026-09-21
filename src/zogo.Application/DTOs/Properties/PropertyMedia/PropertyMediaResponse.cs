namespace zogo.Application.DTOs.Property.PropertyMedia;

public class PropertyMediaResponse
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }

    public short MediaType { get; set; }

    public string StorageKey { get; set; } = null!;

    public string OriginalFileName { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public long FileSizeBytes { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsCover { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }
}