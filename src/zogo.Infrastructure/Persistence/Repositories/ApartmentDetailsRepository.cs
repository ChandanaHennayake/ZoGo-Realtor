using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class ApartmentDetailsRepository
    : IApartmentDetailsRepository
{
    private readonly ApplicationDbContext _context;

    public ApartmentDetailsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        ApartmentDetails apartmentDetails,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(apartmentDetails);

        await _context.ApartmentDetails.AddAsync(
            apartmentDetails,
            cancellationToken);
    }

    public async Task<ApartmentDetails?> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ApartmentDetails
            .FirstOrDefaultAsync(
                x => x.PropertyId == propertyId,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ApartmentDetails
            .AnyAsync(
                x => x.PropertyId == propertyId,
                cancellationToken);
    }
}