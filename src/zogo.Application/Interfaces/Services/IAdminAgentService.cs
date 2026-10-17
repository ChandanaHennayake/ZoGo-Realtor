using zogo.Application.DTOs.Agent;

namespace zogo.Application.Interfaces.Services;

public interface IAdminAgentService
{
    Task<AgentResponse> CreateAgentAsync(CreateAgentRequest request, Guid? adminUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentResponse>> GetAllAgentsAsync(CancellationToken cancellationToken = default);
    Task<AgentResponse> GetAgentByIdAsync(Guid agentId, CancellationToken cancellationToken = default);
    Task<AgentResponse> UpdateAgentAsync(Guid agentId, UpdateAgentRequest request, CancellationToken cancellationToken = default);
    Task<AgentResponse> AssignDistrictsAsync(Guid agentId, AssignAgentDistrictsRequest request, CancellationToken cancellationToken = default);
    Task<AgentResponse> ActivateAgentAsync(Guid agentId, CancellationToken cancellationToken = default);
    Task<AgentResponse> DeactivateAgentAsync(Guid agentId, CancellationToken cancellationToken = default);
}
