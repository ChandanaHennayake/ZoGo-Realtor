namespace zogo.Application.DTOs.Property.PropertyMedia;

public class CreatePropertyMediaRequest
{
    public short MediaType { get; set; }

    public string StorageKey { get; set; } = null!;

    public string OriginalFileName { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public long FileSizeBytes { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsCover { get; set; }
}