using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Buyer;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class PropertyInterestRepository : IPropertyInterestRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyInterestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PropertyInterest propertyInterest, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(propertyInterest);
        await _context.PropertyInterests.AddAsync(propertyInterest, cancellationToken);
    }

    public async Task<PropertyInterest?> GetAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyInterests
            .FirstOrDefaultAsync(x => x.BuyerId == buyerId && x.PropertyId == propertyId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyInterests
            .AnyAsync(x => x.BuyerId == buyerId && x.PropertyId == propertyId, cancellationToken);
    }
}
