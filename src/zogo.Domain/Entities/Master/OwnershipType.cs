namespace zogo.Domain.Entities.Master;

public class OwnershipType
{
    private OwnershipType() { }

    public short Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    public static OwnershipType Create(short id, string code, string name, int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Ownership type code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ownership type name is required.", nameof(name));

        return new OwnershipType
        {
            Id = id,
            Code = code.Trim(),
            Name = name.Trim(),
            IsActive = true,
            DisplayOrder = displayOrder
        };
    }
}
