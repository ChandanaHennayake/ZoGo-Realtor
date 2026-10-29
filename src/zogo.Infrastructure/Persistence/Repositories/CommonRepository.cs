using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence.Repositories;

public class CommonRepository : ICommonRepository
{
    private readonly ApplicationDbContext _context;

    public CommonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Province>> GetProvincesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Provinces
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<District>> GetDistrictsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Districts
            .AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<District>> GetDistrictsByProvinceAsync(
        short provinceId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Districts
            .AsNoTracking()
            .Where(d =>
                d.ProvinceId == provinceId &&
                d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<City>> GetCitiesByDistrictAsync(
        short districtId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Cities
            .AsNoTracking()
            .Where(c =>
                c.DistrictId == districtId &&
                c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DivisionalSecretariat>>
        GetDivisionalSecretariatsByDistrictAsync(
            short districtId,
            CancellationToken cancellationToken = default)
    {
        return await _context.DivisionalSecretariats
            .AsNoTracking()
            .Where(ds =>
                ds.DistrictId == districtId &&
                ds.IsActive)
            .OrderBy(ds => ds.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GramaNiladhariDivision>>
        GetGnDivisionsByDsAsync(
            int divisionalSecretariatId,
            CancellationToken cancellationToken = default)
    {
        return await _context.GramaNiladhariDivisions
            .AsNoTracking()
            .Where(gn =>
                gn.DivisionalSecretariatId == divisionalSecretariatId &&
                gn.IsActive)
            .OrderBy(gn => gn.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PropertyType>> GetPropertyTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.PropertyTypes
            .AsNoTracking()
            .Where(pt => pt.IsActive)
            .OrderBy(pt => pt.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ListingType>> GetListingTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ListingTypes
            .AsNoTracking()
            .Where(lt => lt.IsActive)
            .OrderBy(lt => lt.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ApartmentType>> GetApartmentTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ApartmentTypes
            .AsNoTracking()
            .Where(at => at.IsActive)
            .OrderBy(at => at.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FurnishingType>> GetFurnishingTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.FurnishingTypes
            .AsNoTracking()
            .Where(ft => ft.IsActive)
            .OrderBy(ft => ft.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ViewType>> GetViewTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ViewTypes
            .AsNoTracking()
            .Where(vt => vt.IsActive)
            .OrderBy(vt => vt.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Condominium>> GetCondominiumsAsync(
        short? districtId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Condominiums
            .AsNoTracking()
            .Where(c => c.IsActive);

        if (districtId.HasValue && districtId.Value > 0)
        {
            query = query.Where(c => c.DistrictId == districtId.Value);
        }

        return await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Amenity>> GetAmenitiesAsync(
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Amenities
            .AsNoTracking()
            .Where(a => a.IsActive);

        if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(a => a.Category != null && a.Category.ToLower() == category.ToLower());
        }

        return await query
            .OrderBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Feature>> GetFeaturesAsync(
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Features
            .AsNoTracking()
            .Where(f => f.IsActive);

        if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(f => f.Category != null && f.Category.ToLower() == category.ToLower());
        }

        return await query
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentType>> GetDocumentTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.DocumentTypes
            .AsNoTracking()
            .Where(dt => dt.IsActive)
            .OrderBy(dt => dt.DisplayOrder)
            .ThenBy(dt => dt.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OwnershipType>> GetOwnershipTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.OwnershipTypes
            .AsNoTracking()
            .Where(ot => ot.IsActive)
            .OrderBy(ot => ot.DisplayOrder)
            .ThenBy(ot => ot.Id)
            .ToListAsync(cancellationToken);
    }
}