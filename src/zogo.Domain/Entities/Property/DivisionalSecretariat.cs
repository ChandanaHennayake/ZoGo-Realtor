namespace zogo.Domain.Entities.Property;

public class DivisionalSecretariat
{
    private DivisionalSecretariat() { }

    public int Id { get; private set; }
    public short DistrictId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
}