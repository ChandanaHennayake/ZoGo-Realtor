namespace zogo.Domain.Entities.Identity;

public class UserRole
{
    private UserRole()
    {
    }

    public Guid UserId { get; private set; }

    public short RoleId { get; private set; }

    public DateTime AssignedAt { get; private set; }

    public Guid? AssignedBy { get; private set; }

    public User User { get; private set; } = null!;

    public Role Role { get; private set; } = null!;


    public static UserRole Create(
        Guid userId,
        short roleId,
        Guid? assignedBy)
    {
        return new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy
        };
    }
}