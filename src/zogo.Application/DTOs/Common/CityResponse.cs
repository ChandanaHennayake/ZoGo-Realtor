namespace zogo.Application.DTOs.Common;

public sealed class CityResponse
{
    public int Id { get; set; }
    public short DistrictId { get; set; }
    public string Name { get; set; } = null!;
}
