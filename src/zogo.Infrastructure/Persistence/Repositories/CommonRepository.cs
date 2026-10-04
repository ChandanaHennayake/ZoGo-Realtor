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
}