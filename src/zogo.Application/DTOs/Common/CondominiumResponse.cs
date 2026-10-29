namespace zogo.Application.DTOs.Common;

public sealed class CondominiumResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public short? DistrictId { get; set; }
    public string? DeveloperName { get; set; }
    public int? TotalFloors { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
