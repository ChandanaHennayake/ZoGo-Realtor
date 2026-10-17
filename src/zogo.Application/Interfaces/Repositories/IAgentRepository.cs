using zogo.Application.DTOs.Agent;
using zogo.Domain.Entities.Agent;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Entities.Master;

namespace zogo.Application.Interfaces.Repositories;

public interface IAgentRepository
{
    Task AddProfileAsync(AgentProfile profile, CancellationToken cancellationToken = default);
    Task AddDistrictsAsync(IEnumerable<AgentDistrict> districts, CancellationToken cancellationToken = default);
    Task RemoveDistrictsAsync(Guid agentProfileId, CancellationToken cancellationToken = default);
    Task<AgentProfile?> GetByIdAsync(Guid agentProfileId, CancellationToken cancellationToken = default);
    Task<AgentProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentProfile>> GetActiveAgentsByDistrictIdAsync(short districtId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> CityExistsAsync(int cityId, CancellationToken cancellationToken = default);
    Task<int> CountActiveDistrictsAsync(IEnumerable<short> districtIds, CancellationToken cancellationToken = default);

    Task<AgentDashboardResponse> GetDashboardAsync(IReadOnlyList<short> districtIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentInterestedPropertyResponse>> GetInterestedPropertiesAsync(IReadOnlyList<short> districtIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentPendingVisitResponse>> GetPendingVisitsAsync(IReadOnlyList<short> districtIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentPendingDocumentResponse>> GetPendingDocumentsAsync(IReadOnlyList<short> districtIds, CancellationToken cancellationToken = default);

    Task<Property?> GetPropertyByIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<BuyerPropertyInteraction?> GetInteractionAsync(Guid propertyId, Guid? buyerId, CancellationToken cancellationToken = default);
}
