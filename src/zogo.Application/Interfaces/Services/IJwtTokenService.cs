namespace zogo.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles);

    DateTime GetAccessTokenExpiration();

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);
}