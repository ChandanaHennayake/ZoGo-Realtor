namespace zogo.Domain.Entities.Master;

public class Amenity
{
    private Amenity()
    {
    }

    public int Id { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string? Category { get; private set; }

    public bool IsActive { get; private set; }

    public int DisplayOrder { get; private set; }

    public static Amenity Create(
        string code,
        string name,
        string? category,
        bool isActive,
        int displayOrder)
    {
        return new Amenity
        {
            Code = code.Trim(),
            Name = name.Trim(),
            Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim(),
            IsActive = isActive,
            DisplayOrder = displayOrder
        };
    }
}
