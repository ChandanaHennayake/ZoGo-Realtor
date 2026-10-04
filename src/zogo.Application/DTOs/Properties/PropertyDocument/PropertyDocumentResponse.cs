namespace zogo.Application.DTOs.Property.PropertyDocument;

public class PropertyDocumentResponse
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }

    public int DocumentTypeId { get; set; }

    public string StorageKey { get; set; } = null!;

    public string? OriginalFileName { get; set; }

    public string? MimeType { get; set; }

    public long? FileSizeBytes { get; set; }

    public short Status { get; set; }

    public string StatusName { get; set; } = null!;

    public Guid UploadedBy { get; set; }

    public DateTime UploadedAt { get; set; }

    public Guid? VerifiedBy { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }
}
