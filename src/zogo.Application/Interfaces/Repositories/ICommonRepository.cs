using zogo.Domain.Entities.Master;

namespace zogo.Application.Interfaces.Repositories;

public interface ICommonRepository
{
    Task<IReadOnlyList<District>> GetDistrictsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DivisionalSecretariat>> GetDivisionalSecretariatsByDistrictAsync(short districtId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GramaNiladhariDivision>> GetGnDivisionsByDsAsync(int divisionalSecretariatId, CancellationToken cancellationToken = default);
}
