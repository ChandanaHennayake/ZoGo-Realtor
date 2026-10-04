using zogo.Domain.Enums;

namespace zogo.Domain.Entities.Property;

public class PropertyDocument
{
    private PropertyDocument()
    {
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public int DocumentTypeId { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public string? OriginalFileName { get; private set; }
    public string? MimeType { get; private set; }
    public long? FileSizeBytes { get; private set; }
    public short Status { get; private set; }
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public Guid? VerifiedBy { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public static PropertyDocument Create(
        Guid propertyId,
        int documentTypeId,
        string storageKey,
        string? originalFileName,
        string? mimeType,
        long? fileSizeBytes,
        Guid uploadedBy,
        short status = (short)PropertyDocumentStatus.Uploaded)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        if (documentTypeId <= 0)
            throw new ArgumentException("Document type ID is required.", nameof(documentTypeId));

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("Storage key is required.", nameof(storageKey));

        if (uploadedBy == Guid.Empty)
            throw new ArgumentException("Uploaded by user is required.", nameof(uploadedBy));

        return new PropertyDocument
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            DocumentTypeId = documentTypeId,
            StorageKey = storageKey,
            OriginalFileName = originalFileName,
            MimeType = mimeType,
            FileSizeBytes = fileSizeBytes,
            Status = status,
            UploadedBy = uploadedBy,
            UploadedAt = DateTime.UtcNow
        };
    }

    public void Verify(Guid verifiedBy)
    {
        if (verifiedBy == Guid.Empty)
            throw new ArgumentException("Verified by user is required.", nameof(verifiedBy));

        Status = (short)PropertyDocumentStatus.Verified;
        VerifiedBy = verifiedBy;
        VerifiedAt = DateTime.UtcNow;
        RejectionReason = null;
    }

    public void Reject(Guid verifiedBy, string reason)
    {
        if (verifiedBy == Guid.Empty)
            throw new ArgumentException("Verified by user is required.", nameof(verifiedBy));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required.", nameof(reason));

        Status = (short)PropertyDocumentStatus.Rejected;
        VerifiedBy = verifiedBy;
        VerifiedAt = DateTime.UtcNow;
        RejectionReason = reason;
    }

    public void SetUnderReview(Guid reviewerId)
    {
        Status = (short)PropertyDocumentStatus.UnderReview;
        VerifiedBy = reviewerId;
    }

    public void UpdateStatus(short status, Guid actionUserId, string? rejectionReason = null)
    {
        Status = status;
        if (status == (short)PropertyDocumentStatus.Verified)
        {
            VerifiedBy = actionUserId;
            VerifiedAt = DateTime.UtcNow;
            RejectionReason = null;
        }
        else if (status == (short)PropertyDocumentStatus.Rejected)
        {
            VerifiedBy = actionUserId;
            VerifiedAt = DateTime.UtcNow;
            RejectionReason = rejectionReason;
        }
        else if (status == (short)PropertyDocumentStatus.UnderReview)
        {
            VerifiedBy = actionUserId;
            VerifiedAt = null;
            RejectionReason = null;
        }
        else if (status == (short)PropertyDocumentStatus.Uploaded)
        {
            VerifiedBy = null;
            VerifiedAt = null;
            RejectionReason = null;
        }
    }
}
