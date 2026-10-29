namespace zogo.Application.DTOs.Common;

public sealed class ViewTypeResponse
{
    public short Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
