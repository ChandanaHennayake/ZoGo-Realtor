using zogo.Application.DTOs.Agent;
using zogo.Application.DTOs.Buyer;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Agent;
using zogo.Domain.Enums;

namespace zogo.Application.Services;

public sealed class AgentService : IAgentService
{
    private readonly IAgentRepository _agentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AgentService(
        IAgentRepository agentRepository,
        IUnitOfWork unitOfWork)
    {
        _agentRepository = agentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AgentDashboardResponse> GetDashboardAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var agent = await GetActiveAgentOrThrowAsync(userId, cancellationToken);
        var districtIds = agent.AgentDistricts.Select(d => d.DistrictId).ToList();

        return await _agentRepository.GetDashboardAsync(districtIds, cancellationToken);
    }

    public async Task<IReadOnlyList<AgentInterestedPropertyResponse>> GetInterestedPropertiesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var agent = await GetActiveAgentOrThrowAsync(userId, cancellationToken);
        var districtIds = agent.AgentDistricts.Select(d => d.DistrictId).ToList();

        return await _agentRepository.GetInterestedPropertiesAsync(districtIds, cancellationToken);
    }

    public async Task<IReadOnlyList<AgentPendingVisitResponse>> GetPendingVisitsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var agent = await GetActiveAgentOrThrowAsync(userId, cancellationToken);
        var districtIds = agent.AgentDistricts.Select(d => d.DistrictId).ToList();

        return await _agentRepository.GetPendingVisitsAsync(districtIds, cancellationToken);
    }

    public async Task<BuyerPropertyInteractionResponse> MarkVisitedAsync(
        Guid userId,
        Guid propertyId,
        Guid? buyerId = null,
        CancellationToken cancellationToken = default)
    {
        var agent = await GetActiveAgentOrThrowAsync(userId, cancellationToken);
        var districtIds = agent.AgentDistricts.Select(d => d.DistrictId).ToList();

        var property = await _agentRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
        if (property is null)
            throw new NotFoundException("Property", propertyId);

        // STEP 10 & 14: Verify property belongs to one of Agent's assigned districts
        if (!districtIds.Contains(property.DistrictId))
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to manage properties outside your assigned operating districts.");
        }

        var interaction = await _agentRepository.GetInteractionAsync(propertyId, buyerId, cancellationToken);
        if (interaction is null)
            throw new KeyNotFoundException("No buyer interaction found for this property.");

        // STEP 10: Verify current status is 2 (VISIT_REQUESTED). Reject 1->3, 4->3, etc.
        if (interaction.CurrentStatus != (short)BuyerPropertyInteractionStatus.VisitRequested)
        {
            throw new InvalidOperationException(
                $"Cannot mark visited. Current interaction status must be 2 (VISIT_REQUESTED), but is {interaction.CurrentStatus}.");
        }

        interaction.UpdateCurrentStatus((short)BuyerPropertyInteractionStatus.Visited);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BuyerPropertyInteractionResponse
        {
            Id = interaction.Id,
            PropertyId = interaction.PropertyId,
            BuyerId = interaction.BuyerId,
            CurrentStatus = interaction.CurrentStatus,
            CurrentStatusName = "VISITED"
        };
    }

    public async Task<IReadOnlyList<AgentPendingDocumentResponse>> GetPendingDocumentsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var agent = await GetActiveAgentOrThrowAsync(userId, cancellationToken);
        var districtIds = agent.AgentDistricts.Select(d => d.DistrictId).ToList();

        return await _agentRepository.GetPendingDocumentsAsync(districtIds, cancellationToken);
    }

    private async Task<AgentProfile> GetActiveAgentOrThrowAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var agent = await _agentRepository.GetByUserIdAsync(userId, cancellationToken);
        if (agent is null)
            throw new UnauthorizedAccessException("Agent profile not found for this account.");

        if (!agent.IsActive)
            throw new UnauthorizedAccessException("Agent profile is inactive. Contact administrator.");

        return agent;
    }
}
