namespace zogo.Application.DTOs.Common;

public sealed class DocumentTypeResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int DisplayOrder { get; set; }
}
