namespace zogo.Application.DTOs.Common;

public sealed class FurnishingTypeResponse
{
    public short Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
