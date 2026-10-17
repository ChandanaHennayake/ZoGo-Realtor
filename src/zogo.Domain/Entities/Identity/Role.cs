namespace zogo.Domain.Entities.Identity;

public class Role
{
    private Role()
    {
    }

    public short Id { get; private set; }

    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; }
        = new List<UserRole>();

    public static Role Create(string code, string name, bool isActive = true, short id = 0)
    {
        return new Role
        {
            Id = id,
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            IsActive = isActive
        };
    }
}
