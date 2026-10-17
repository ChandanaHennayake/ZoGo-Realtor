using zogo.Application.DTOs.Agent;
using zogo.Application.DTOs.Buyer;

namespace zogo.Application.Interfaces.Services;

public interface IAgentService
{
    Task<AgentDashboardResponse> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentInterestedPropertyResponse>> GetInterestedPropertiesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentPendingVisitResponse>> GetPendingVisitsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BuyerPropertyInteractionResponse> MarkVisitedAsync(Guid userId, Guid propertyId, Guid? buyerId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentPendingDocumentResponse>> GetPendingDocumentsAsync(Guid userId, CancellationToken cancellationToken = default);
}
