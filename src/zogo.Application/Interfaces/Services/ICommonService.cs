using zogo.Application.DTOs.Common;

namespace zogo.Application.Interfaces.Services;

public interface ICommonService
{
    Task<IReadOnlyList<ProvinceResponse>> GetProvincesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DistrictResponse>> GetDistrictsAsync(short? provinceId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CityResponse>> GetCitiesByDistrictAsync(short districtId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DivisionalSecretariatResponse>> GetDivisionalSecretariatsAsync(short districtId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GramaNiladhariDivisionResponse>> GetGramaNiladhariDivisionsAsync(int divisionalSecretariatId, CancellationToken cancellationToken = default);
}
