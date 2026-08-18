namespace zogo.Application.DTOs.Authentication;

public sealed class GoogleLoginRequest
{
    public string IdToken { get; init; } = string.Empty;
}