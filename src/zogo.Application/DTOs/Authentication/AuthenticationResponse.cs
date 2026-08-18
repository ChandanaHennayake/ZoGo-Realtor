namespace zogo.Application.DTOs.Authentication;

public sealed class AuthenticationResponse
{
    public Guid UserId { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string AccessToken { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;

    public DateTime AccessTokenExpiresAt { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; }
        = Array.Empty<string>();
}