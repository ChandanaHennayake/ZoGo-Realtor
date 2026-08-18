using zogo.Domain.Enums;

namespace zogo.Domain.Entities.Identity;

public class DeviceToken
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Token { get; private set; } = null!;

    public DevicePlatform Platform { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? LastUsedAt { get; private set; }

    public User User { get; private set; } = null!;
}

public enum DevicePlatform : short { Android = 1, IOS = 2 }