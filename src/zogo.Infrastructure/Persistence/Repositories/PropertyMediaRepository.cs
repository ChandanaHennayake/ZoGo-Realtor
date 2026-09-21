using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Repositories;

public class PropertyMediaRepository : IPropertyMediaRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyMediaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        PropertyMedia propertyMedia,
        CancellationToken cancellationToken = default)
    {
        await _context.PropertyMedia.AddAsync(
            propertyMedia,
            cancellationToken);
    }

    public async Task<PropertyMedia?> GetByIdAsync(
        Guid mediaId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyMedia
            .FirstOrDefaultAsync(
                x => x.Id == mediaId,
                cancellationToken);
    }

    public async Task<List<PropertyMedia>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyMedia
            .Where(x => x.PropertyId == propertyId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid mediaId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyMedia
            .AnyAsync(
                x => x.Id == mediaId,
                cancellationToken);
    }

    public Task DeleteAsync(
        PropertyMedia propertyMedia,
        CancellationToken cancellationToken = default)
    {
        _context.PropertyMedia.Remove(propertyMedia);

        return Task.CompletedTask;
    }
}