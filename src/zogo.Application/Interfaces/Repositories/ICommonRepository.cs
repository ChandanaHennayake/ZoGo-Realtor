using zogo.Domain.Entities.Master;

namespace zogo.Application.Interfaces.Repositories;

public interface ICommonRepository
{
    Task<IReadOnlyList<Province>> GetProvincesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<District>> GetDistrictsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<District>> GetDistrictsByProvinceAsync(
        short provinceId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<City>> GetCitiesByDistrictAsync(
        short districtId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DivisionalSecretariat>>
        GetDivisionalSecretariatsByDistrictAsync(
            short districtId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GramaNiladhariDivision>>
        GetGnDivisionsByDsAsync(
            int divisionalSecretariatId,
            CancellationToken cancellationToken = default);
}