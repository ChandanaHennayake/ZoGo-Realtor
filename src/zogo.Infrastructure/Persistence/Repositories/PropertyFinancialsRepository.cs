using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Property;
using zogo.Infrastructure.Persistence;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class PropertyFinancialsRepository
    : IPropertyFinancialsRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyFinancialsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        PropertyFinancials propertyFinancials,
        CancellationToken cancellationToken = default)
    {
        await _context.PropertyFinancials.AddAsync(
            propertyFinancials,
            cancellationToken);
    }

    public async Task<PropertyFinancials?> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyFinancials
            .FirstOrDefaultAsync(
                x => x.PropertyId == propertyId,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyFinancials
            .AnyAsync(
                x => x.PropertyId == propertyId,
                cancellationToken);
    }
}