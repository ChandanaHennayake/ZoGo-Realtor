using Microsoft.AspNetCore.Http;

namespace zogo.Application.DTOs.Property.PropertyMedia;

public class UploadPropertyMediaRequest
{
    public IFormFile File { get; set; } = null!;

    public short MediaType { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsCover { get; set; }
}