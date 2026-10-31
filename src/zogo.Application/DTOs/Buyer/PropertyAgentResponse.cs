namespace zogo.Application.DTOs.Buyer;

public sealed class PropertyAgentResponse
{
    public Guid AgentId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public short DistrictId { get; set; }
    public string? DistrictName { get; set; }
}

