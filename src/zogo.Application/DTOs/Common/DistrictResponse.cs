namespace zogo.Application.DTOs.Common;

public sealed class DistrictResponse
{
    public short Id { get; set; }
    public short ProvinceId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
