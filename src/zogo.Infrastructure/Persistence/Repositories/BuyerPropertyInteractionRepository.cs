using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Buyer;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class BuyerPropertyInteractionRepository : IBuyerPropertyInteractionRepository
{
    private readonly ApplicationDbContext _context;

    public BuyerPropertyInteractionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(BuyerPropertyInteraction interaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(interaction);
        await _context.BuyerPropertyInteractions.AddAsync(interaction, cancellationToken);
    }

    public async Task<BuyerPropertyInteraction?> GetAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.BuyerPropertyInteractions
            .FirstOrDefaultAsync(x => x.BuyerId == buyerId && x.PropertyId == propertyId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid buyerId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.BuyerPropertyInteractions
            .AnyAsync(x => x.BuyerId == buyerId && x.PropertyId == propertyId, cancellationToken);
    }
}
