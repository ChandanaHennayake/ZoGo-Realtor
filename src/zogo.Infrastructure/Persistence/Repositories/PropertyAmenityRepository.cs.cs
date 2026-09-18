using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Repositories;

public class PropertyAmenityRepository : IPropertyAmenityRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyAmenityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        PropertyAmenity propertyAmenity,
        CancellationToken cancellationToken = default)
    {
        await _context.PropertyAmenities.AddAsync(
            propertyAmenity,
            cancellationToken);
    }

    public async Task<List<PropertyAmenity>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyAmenities
            .Include(x => x.Amenity)
            .Where(x => x.PropertyId == propertyId)
            .OrderBy(x => x.Amenity.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<PropertyAmenity?> GetAsync(
        Guid propertyId,
        int amenityId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyAmenities
            .Include(x => x.Amenity)
            .FirstOrDefaultAsync(
                x => x.PropertyId == propertyId &&
                     x.AmenityId == amenityId,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid propertyId,
        int amenityId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyAmenities
            .AnyAsync(
                x => x.PropertyId == propertyId &&
                     x.AmenityId == amenityId,
                cancellationToken);
    }

    public Task DeleteAsync(
        PropertyAmenity propertyAmenity,
        CancellationToken cancellationToken = default)
    {
        _context.PropertyAmenities.Remove(propertyAmenity);

        return Task.CompletedTask;
    }
}