namespace zogo.Application.DTOs.Property.PropertyDocument;

public class UpdatePropertyDocumentStatusRequest
{
    public short Status { get; set; }

    public string? RejectionReason { get; set; }
}
