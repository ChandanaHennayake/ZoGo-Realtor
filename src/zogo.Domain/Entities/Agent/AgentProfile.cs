using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;

namespace zogo.Domain.Entities.Agent;

public class AgentProfile
{
    private readonly List<AgentDistrict> _agentDistricts = new();

    private AgentProfile()
    {
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int CityId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public User User { get; private set; } = null!;
    public City City { get; private set; } = null!;
    public IReadOnlyCollection<AgentDistrict> AgentDistricts => _agentDistricts.AsReadOnly();

    public static AgentProfile Create(Guid userId, int cityId, bool isActive = true)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        if (cityId <= 0)
            throw new ArgumentException("City ID is required.", nameof(cityId));

        return new AgentProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CityId = cityId,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(int cityId, bool? isActive = null)
    {
        if (cityId <= 0)
            throw new ArgumentException("City ID is required.", nameof(cityId));

        CityId = cityId;

        if (isActive.HasValue)
        {
            IsActive = isActive.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
