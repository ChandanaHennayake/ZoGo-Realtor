using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Repositories;

public class PropertyDocumentRepository : IPropertyDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyDocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        PropertyDocument propertyDocument,
        CancellationToken cancellationToken = default)
    {
        await _context.PropertyDocuments.AddAsync(
            propertyDocument,
            cancellationToken);
    }

    public async Task<PropertyDocument?> GetByIdAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyDocuments
            .FirstOrDefaultAsync(
                x => x.Id == documentId,
                cancellationToken);
    }

    public async Task<List<PropertyDocument>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyDocuments
            .Where(x => x.PropertyId == propertyId)
            .OrderByDescending(x => x.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyDocuments
            .AnyAsync(
                x => x.Id == documentId,
                cancellationToken);
    }

    public Task DeleteAsync(
        PropertyDocument propertyDocument,
        CancellationToken cancellationToken = default)
    {
        _context.PropertyDocuments.Remove(propertyDocument);

        return Task.CompletedTask;
    }
}
