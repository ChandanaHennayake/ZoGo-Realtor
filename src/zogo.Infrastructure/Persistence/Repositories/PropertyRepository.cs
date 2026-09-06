using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class PropertyRepository : IPropertyRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Property property,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(property);

        await _context.Properties.AddAsync(
            property,
            cancellationToken);
    }

    public async Task<Property?> GetByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .FirstOrDefaultAsync(
                x =>
                    x.Id == propertyId &&
                    x.DeletedAt == null,
                cancellationToken);
    }
}