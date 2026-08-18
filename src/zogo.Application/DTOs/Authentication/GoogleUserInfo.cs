namespace zogo.Application.DTOs.Authentication;

public sealed class GoogleUserInfo
{
    public string Subject { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public bool EmailVerified { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? ProfileImageUrl { get; init; }
}