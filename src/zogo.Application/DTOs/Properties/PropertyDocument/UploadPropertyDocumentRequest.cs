using Microsoft.AspNetCore.Http;

namespace zogo.Application.DTOs.Property.PropertyDocument;

public class UploadPropertyDocumentRequest
{
    public IFormFile File { get; set; } = null!;

    public int DocumentTypeId { get; set; }
}
