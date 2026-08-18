namespace zogo.Domain.Entities.Identity;

public class Role
{
    public short Id { get; private set; }

    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; }
        = new List<UserRole>();
}