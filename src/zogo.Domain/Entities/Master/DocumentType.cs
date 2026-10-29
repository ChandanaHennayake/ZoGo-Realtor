namespace zogo.Domain.Entities.Master;

public class DocumentType
{
    private DocumentType() { }

    public int Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    public static DocumentType Create(string code, string name, int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Document type code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Document type name is required.", nameof(name));

        return new DocumentType
        {
            Code = code.Trim(),
            Name = name.Trim(),
            IsActive = true,
            DisplayOrder = displayOrder
        };
    }
}
