using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Identity;
using zogo.Domain.Enums;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(
    string email,
    CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _context.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(
                x =>
                    x.Email == normalizedEmail &&
                    x.DeletedAt == null,
                cancellationToken);
    }

    public async Task<UserAuthenticationProvider?> GetLocalProviderAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        return await _context.UserAuthenticationProviders
            .FirstOrDefaultAsync(
                x => x.UserId == userId &&
                     x.Provider == AuthenticationProvider.Local,
                cancellationToken);
    }


    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(
            user,
            cancellationToken);
    }
}