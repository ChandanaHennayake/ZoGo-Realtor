namespace zogo.Application.DTOs.Common;

public sealed class DivisionalSecretariatResponse
{
    public int Id { get; set; }
    public short DistrictId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
