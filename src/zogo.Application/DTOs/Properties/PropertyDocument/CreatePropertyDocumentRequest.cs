namespace zogo.Application.DTOs.Property.PropertyDocument;

public class CreatePropertyDocumentRequest
{
    public int DocumentTypeId { get; set; }

    public string StorageKey { get; set; } = null!;

    public string? OriginalFileName { get; set; }

    public string? MimeType { get; set; }

    public long? FileSizeBytes { get; set; }
}
