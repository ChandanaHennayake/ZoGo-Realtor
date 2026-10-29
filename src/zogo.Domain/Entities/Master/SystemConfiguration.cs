namespace zogo.Domain.Entities.Master;

public class SystemConfiguration
{
    public int Id { get; set; }

    public string ConfigKey { get; set; } = null!;

    public string ConfigValue { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime UpdatedAt { get; set; }
}
