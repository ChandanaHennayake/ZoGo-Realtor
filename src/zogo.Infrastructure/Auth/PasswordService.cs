using zogo.Application.Interfaces.Services;

namespace zogo.Infrastructure.Auth;

public sealed class PasswordService : IPasswordService
{
    private const int WorkFactor = 12;


    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException(
                "Password is required.",
                nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(
            password,
            WorkFactor);
    }


    public bool VerifyPassword(
        string password,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(passwordHash))
            return false;

        return BCrypt.Net.BCrypt.Verify(
            password,
            passwordHash);
    }
}