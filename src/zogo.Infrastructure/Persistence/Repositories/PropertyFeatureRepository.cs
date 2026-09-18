using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Repositories;

public class PropertyFeatureRepository : IPropertyFeatureRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyFeatureRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        PropertyFeature propertyFeature,
        CancellationToken cancellationToken = default)
    {
        await _context.PropertyFeatures.AddAsync(
            propertyFeature,
            cancellationToken);
    }

    public async Task<List<PropertyFeature>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyFeatures
            .Include(x => x.Feature)
            .Where(x => x.PropertyId == propertyId)
            .OrderBy(x => x.Feature.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<PropertyFeature?> GetAsync(
        Guid propertyId,
        int featureId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyFeatures
            .FirstOrDefaultAsync(
                x => x.PropertyId == propertyId &&
                     x.FeatureId == featureId,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid propertyId,
        int featureId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyFeatures
            .AnyAsync(
                x => x.PropertyId == propertyId &&
                     x.FeatureId == featureId,
                cancellationToken);
    }

    public Task DeleteAsync(
        PropertyFeature propertyFeature,
        CancellationToken cancellationToken = default)
    {
        _context.PropertyFeatures.Remove(propertyFeature);

        return Task.CompletedTask;
    }
}