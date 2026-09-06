namespace zogo.Domain.Entities.Property;

public class GramaNiladhariDivision
{
    private GramaNiladhariDivision() { }

    public int Id { get; private set; }
    public int DivisionalSecretariatId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
}