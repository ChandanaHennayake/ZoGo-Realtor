using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class PropertyLegalDetailsRepository
    : IPropertyLegalDetailsRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyLegalDetailsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        PropertyLegalDetails legalDetails,
        CancellationToken cancellationToken = default)
    {
        await _context.PropertyLegalDetails.AddAsync(
            legalDetails,
            cancellationToken);
    }

    public async Task<PropertyLegalDetails?> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyLegalDetails
            .FirstOrDefaultAsync(
                x => x.PropertyId == propertyId,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyLegalDetails
            .AnyAsync(
                x => x.PropertyId == propertyId,
                cancellationToken);
    }
}