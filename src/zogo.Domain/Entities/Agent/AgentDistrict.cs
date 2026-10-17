using zogo.Domain.Entities.Master;

namespace zogo.Domain.Entities.Agent;

public class AgentDistrict
{
    private AgentDistrict()
    {
    }

    public Guid Id { get; private set; }
    public Guid AgentId { get; private set; }
    public short DistrictId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AgentProfile AgentProfile { get; private set; } = null!;
    public District District { get; private set; } = null!;

    public static AgentDistrict Create(Guid agentId, short districtId)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("Agent ID is required.", nameof(agentId));

        if (districtId <= 0)
            throw new ArgumentException("District ID is required.", nameof(districtId));

        return new AgentDistrict
        {
            Id = Guid.NewGuid(),
            AgentId = agentId,
            DistrictId = districtId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
