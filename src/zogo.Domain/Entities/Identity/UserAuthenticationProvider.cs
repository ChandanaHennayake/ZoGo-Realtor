using zogo.Domain.Enums;

namespace zogo.Domain.Entities.Identity;

public class UserAuthenticationProvider
{
    private UserAuthenticationProvider()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public AuthenticationProvider Provider { get; private set; }

    public string? ProviderUserId { get; private set; }

    public string? ProviderEmail { get; private set; }

    public string? PasswordHash { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? LastLoginAt { get; private set; }

    public User User { get; private set; } = null!;


    public static UserAuthenticationProvider CreateLocal(
        Guid userId,
        string email,
        string passwordHash)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "Email is required.",
                nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException(
                "Password hash is required.",
                nameof(passwordHash));

        return new UserAuthenticationProvider
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Provider = AuthenticationProvider.Local,
            ProviderEmail = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }


    public static UserAuthenticationProvider CreateGoogle(
        Guid userId,
        string providerUserId,
        string email)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(providerUserId))
            throw new ArgumentException(
                "Google provider user ID is required.",
                nameof(providerUserId));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "Email is required.",
                nameof(email));

        return new UserAuthenticationProvider
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Provider = AuthenticationProvider.Google,
            ProviderUserId = providerUserId.Trim(),
            ProviderEmail = email.Trim().ToLowerInvariant(),
            PasswordHash = null,
            CreatedAt = DateTime.UtcNow
        };
    }


    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}