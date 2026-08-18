using zogo.Application.DTOs.Authentication;

namespace zogo.Application.Interfaces.Services;

public interface IGoogleAuthenticationService
{
    Task<GoogleUserInfo?> ValidateTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default);
}