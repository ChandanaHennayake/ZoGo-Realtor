namespace zogo.Domain.Entities.Master;

public class Feature
{
    private Feature()
    {
    }

    public int Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Category { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    public static Feature Create(
        string code,
        string name,
        string? category,
        bool isActive,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Feature code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Feature name is required.",
                nameof(name));

        if (displayOrder < 0)
            throw new ArgumentException(
                "Display order cannot be negative.",
                nameof(displayOrder));

        return new Feature
        {
            Code = code.Trim(),
            Name = name.Trim(),
            Category = string.IsNullOrWhiteSpace(category)
                ? null
                : category.Trim(),
            IsActive = isActive,
            DisplayOrder = displayOrder
        };
    }

    public void Update(
        string code,
        string name,
        string? category,
        bool isActive,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Feature code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Feature name is required.",
                nameof(name));

        if (displayOrder < 0)
            throw new ArgumentException(
                "Display order cannot be negative.",
                nameof(displayOrder));

        Code = code.Trim();
        Name = name.Trim();
        Category = string.IsNullOrWhiteSpace(category)
            ? null
            : category.Trim();
        IsActive = isActive;
        DisplayOrder = displayOrder;
    }
}