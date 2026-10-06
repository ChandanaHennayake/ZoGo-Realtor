namespace zogo.Application.DTOs.Buyer;

public sealed class PropertyInterestResponse
{
    public Guid PropertyId { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
