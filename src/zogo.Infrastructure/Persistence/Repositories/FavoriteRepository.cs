using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class FavoriteRepository : IFavoriteRepository
{
    private readonly ApplicationDbContext _context;

    public FavoriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(favorite);
        await _context.Favorites.AddAsync(favorite, cancellationToken);
    }

    public Task DeleteAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(favorite);
        _context.Favorites.Remove(favorite);
        return Task.CompletedTask;
    }

    public async Task<Favorite?> GetAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PropertyId == propertyId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .AnyAsync(x => x.UserId == userId && x.PropertyId == propertyId, cancellationToken);
    }

    public async Task<IReadOnlyList<Property>> GetFavoritePropertiesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .AsNoTracking()
            .Where(f => f.UserId == userId && f.Property != null && f.Property.Status == 2 && f.Property.DeletedAt == null)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => f.Property!)
            .ToListAsync(cancellationToken);
    }
}
