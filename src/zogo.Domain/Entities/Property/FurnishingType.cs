namespace zogo.Domain.Entities.Property;

public class FurnishingType
{
    private FurnishingType()
    {
    }

    public short Id { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public static FurnishingType Create(
        string code,
        string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Furnishing type code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Furnishing type name is required.",
                nameof(name));

        return new FurnishingType
        {
            Code = code.Trim(),
            Name = name.Trim(),
            IsActive = true
        };
    }

    public void Update(
        string code,
        string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Furnishing type code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Furnishing type name is required.",
                nameof(name));

        Code = code.Trim();
        Name = name.Trim();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}