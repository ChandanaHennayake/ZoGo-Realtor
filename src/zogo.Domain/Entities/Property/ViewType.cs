namespace zogo.Domain.Entities.Property;

public class ViewType
{
    private ViewType()
    {
    }

    public short Id { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public static ViewType Create(
        string code,
        string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "View type code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "View type name is required.",
                nameof(name));

        return new ViewType
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
                "View type code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "View type name is required.",
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