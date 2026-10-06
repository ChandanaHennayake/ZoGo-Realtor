namespace zogo.Domain.Entities.Master;

public class PropertyStatus
{
    public short Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
