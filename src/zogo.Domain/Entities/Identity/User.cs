using zogo.Domain.Enums;

namespace zogo.Domain.Entities.Identity;

public class User
{
    private readonly List<UserAuthenticationProvider> _authenticationProviders = new();
    private readonly List<UserRole> _userRoles = new();
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<PasswordReset> _passwordResets = new();
    private readonly List<DeviceToken> _deviceTokens = new();

    private User()
    {
    }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string? PhoneNumber { get; private set; }

    public string? ProfileImageUrl { get; private set; }

    public bool EmailVerified { get; private set; }

    public bool PhoneVerified { get; private set; }

    public UserStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyCollection<UserAuthenticationProvider> AuthenticationProviders =>
        _authenticationProviders.AsReadOnly();

    public IReadOnlyCollection<UserRole> UserRoles =>
        _userRoles.AsReadOnly();

    public IReadOnlyCollection<RefreshToken> RefreshTokens =>
        _refreshTokens.AsReadOnly();

    public IReadOnlyCollection<PasswordReset> PasswordResets =>
        _passwordResets.AsReadOnly();

    public IReadOnlyCollection<DeviceToken> DeviceTokens =>
        _deviceTokens.AsReadOnly();


    public static User Create(
        string firstName,
        string lastName,
        string email,
        string? phoneNumber = null,
        string? profileImageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber)
                ? null
                : phoneNumber.Trim(),
            ProfileImageUrl = string.IsNullOrWhiteSpace(profileImageUrl)
                ? null
                : profileImageUrl.Trim(),
            EmailVerified = false,
            PhoneVerified = false,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }


    public void AddAuthenticationProvider(
        UserAuthenticationProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        _authenticationProviders.Add(provider);
    }


    public void AddRole(UserRole userRole)
    {
        ArgumentNullException.ThrowIfNull(userRole);

        _userRoles.Add(userRole);
    }


    public void MarkEmailAsVerified()
    {
        EmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }


    public void MarkPhoneAsVerified()
    {
        PhoneVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }


    public void UpdateProfile(
        string firstName,
        string lastName,
        string? phoneNumber)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber)
            ? null
            : phoneNumber.Trim();

        UpdatedAt = DateTime.UtcNow;
    }


    public void ChangeStatus(UserStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }


    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }
}