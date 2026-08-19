using zogo.Application.DTOs.Authentication;

namespace zogo.Application.Interfaces.Services;

public interface IAuthenticationService
{
    Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);
    Task<AuthenticationResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
    Task<AuthenticationResponse> GoogleLoginAsync(
    GoogleLoginRequest request,
    CancellationToken cancellationToken = default);
}