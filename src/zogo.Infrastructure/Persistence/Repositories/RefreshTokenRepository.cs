using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Identity;
using zogo.Infrastructure.Persistence;

namespace zogo.Infrastructure.Persistence.Repositories.Identity;

public sealed class RefreshTokenRepository
    : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);
    }

    public Task RevokeAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        refreshToken.Revoke();

        return Task.CompletedTask;
    }
}