namespace zogo.Domain.Entities.Property;

public class District
{
    private District() { }

    public short Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
}