using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using zogo.Application.DTOs.Authentication;
using zogo.Application.Interfaces.Services;

namespace zogo.Infrastructure.Auth;

public sealed class GoogleAuthenticationService
    : IGoogleAuthenticationService
{
    private readonly string _clientId;

    public GoogleAuthenticationService(
        IConfiguration configuration)
    {
        _clientId = configuration["Google:ClientId"]
            ?? throw new InvalidOperationException(
                "Google:ClientId is not configured.");
    }

    public async Task<GoogleUserInfo?> ValidateTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            return null;
        }

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _clientId }
                });

            return new GoogleUserInfo
            {
                Subject = payload.Subject,
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                FirstName = payload.GivenName ?? string.Empty,
                LastName = payload.FamilyName ?? string.Empty,
                ProfileImageUrl = payload.Picture
            };
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}