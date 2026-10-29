namespace zogo.Application.DTOs.Common;

public sealed class FeatureMasterResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Category { get; set; }
    public int DisplayOrder { get; set; }
}
