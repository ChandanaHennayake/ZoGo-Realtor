using zogo.Application.DTOs.Visits;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Enums;

namespace zogo.Application.Services;

public sealed class PropertyVisitService : IPropertyVisitService
{
    private readonly IPropertyVisitRepository _visitRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IBuyerPropertyInteractionRepository _interactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyVisitService(
        IPropertyVisitRepository visitRepository,
        IPropertyRepository propertyRepository,
        IAgentRepository agentRepository,
        IBuyerPropertyInteractionRepository interactionRepository,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _propertyRepository = propertyRepository;
        _agentRepository = agentRepository;
        _interactionRepository = interactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VisitResponse> ScheduleVisitAsync(
        Guid buyerId,
        CreateVisitRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.PropertyId == Guid.Empty)
            throw new ValidationException("PropertyId", "Property ID is required.");

        var scheduledAtUtc = request.ScheduledAt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.ScheduledAt, DateTimeKind.Utc)
            : request.ScheduledAt.ToUniversalTime();

        if (scheduledAtUtc.Date < DateTime.UtcNow.Date)
            throw new ValidationException("ScheduledAt", "Visit date cannot be in the past.");

        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property == null || property.DeletedAt.HasValue)
            throw new NotFoundException("Property", request.PropertyId);

        // A property can only be visited if it is Published (Status = 2)
        if (property.Status != 2)
            throw new InvalidOperationException("Visits can only be scheduled for published listings.");

        var visit = PropertyVisit.Create(
            request.PropertyId,
            buyerId,
            scheduledAtUtc,
            request.TimeSlot,
            request.BuyerNotes);

        await _visitRepository.AddAsync(visit, cancellationToken);

        // Update or create buyer interaction status to VisitRequested (2)
        var interaction = await _interactionRepository.GetAsync(buyerId, request.PropertyId, cancellationToken);
        if (interaction != null)
        {
            interaction.UpdateCurrentStatus((short)BuyerPropertyInteractionStatus.VisitRequested);
        }
        else
        {
            var newInteraction = BuyerPropertyInteraction.Create(
                request.PropertyId,
                buyerId,
                (short)BuyerPropertyInteractionStatus.VisitRequested);
            await _interactionRepository.AddAsync(newInteraction, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _visitRepository.GetByIdAsync(visit.Id, cancellationToken);
        return MapSingle(created!);
    }

    public async Task<IReadOnlyList<VisitResponse>> GetMyVisitsAsync(
        Guid buyerId,
        CancellationToken cancellationToken = default)
    {
        return await _visitRepository.GetVisitsByBuyerIdAsync(buyerId, cancellationToken);
    }

    public async Task<IReadOnlyList<VisitResponse>> GetPropertyVisitsAsync(
        Guid propertyId,
        Guid requestingUserId,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property == null)
            throw new NotFoundException("Property", propertyId);

        return await _visitRepository.GetVisitsByPropertyIdAsync(propertyId, cancellationToken);
    }

    public async Task<IReadOnlyList<VisitResponse>> GetAgentVisitsAsync(
        Guid agentUserId,
        CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByUserIdAsync(agentUserId, cancellationToken);
        if (agent == null)
            throw new NotFoundException("AgentProfile", agentUserId);

        var districtIds = agent.AgentDistricts.Select(d => d.DistrictId).ToList();
        return await _visitRepository.GetVisitsByAgentDistrictsAsync(districtIds, cancellationToken);
    }

    public async Task<IReadOnlyList<VisitResponse>> GetAllVisitsAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 20 : pageSize;
        return await _visitRepository.GetAllVisitsAsync(status, page, size, cancellationToken);
    }

    public async Task<VisitResponse> ConfirmVisitAsync(
        Guid visitId,
        ConfirmVisitRequest request,
        Guid confirmedByUserId,
        CancellationToken cancellationToken = default)
    {
        var visit = await _visitRepository.GetByIdAsync(visitId, cancellationToken);
        if (visit == null || visit.DeletedAt.HasValue)
            throw new NotFoundException("PropertyVisit", visitId);

        visit.Confirm(request.AssignedAgentId ?? confirmedByUserId, request.Notes);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapSingle(visit);
    }

    public async Task<VisitResponse> CompleteVisitAsync(
        Guid visitId,
        CompleteVisitRequest request,
        Guid completedByUserId,
        CancellationToken cancellationToken = default)
    {
        var visit = await _visitRepository.GetByIdAsync(visitId, cancellationToken);
        if (visit == null || visit.DeletedAt.HasValue)
            throw new NotFoundException("PropertyVisit", visitId);

        visit.Complete(request.Notes);

        // Update interaction to Visited (3)
        var interaction = await _interactionRepository.GetAsync(visit.BuyerId, visit.PropertyId, cancellationToken);
        if (interaction != null)
        {
            interaction.UpdateCurrentStatus((short)BuyerPropertyInteractionStatus.Visited);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapSingle(visit);
    }

    public async Task<VisitResponse> CancelVisitAsync(
        Guid visitId,
        CancelVisitRequest request,
        Guid cancelledByUserId,
        CancellationToken cancellationToken = default)
    {
        var visit = await _visitRepository.GetByIdAsync(visitId, cancellationToken);
        if (visit == null || visit.DeletedAt.HasValue)
            throw new NotFoundException("PropertyVisit", visitId);

        visit.Cancel(request.Reason);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapSingle(visit);
    }

    private static VisitResponse MapSingle(PropertyVisit v)
    {
        return new VisitResponse
        {
            VisitId = v.Id,
            PropertyId = v.PropertyId,
            PropertyTitle = v.Property?.Title ?? "Property Listing",
            PropertyReferenceNo = v.Property?.ReferenceNo ?? string.Empty,
            DistrictName = string.Empty,
            CityName = string.Empty,
            BuyerId = v.BuyerId,
            BuyerName = v.Buyer != null ? $"{v.Buyer.FirstName} {v.Buyer.LastName}".Trim() : "Buyer",
            BuyerEmail = v.Buyer?.Email ?? string.Empty,
            BuyerPhone = v.Buyer?.PhoneNumber ?? string.Empty,
            AssignedAgentId = v.AssignedAgentId,
            AssignedAgentName = v.AssignedAgent != null ? $"{v.AssignedAgent.FirstName} {v.AssignedAgent.LastName}".Trim() : null,
            ScheduledAt = v.ScheduledAt,
            TimeSlot = v.TimeSlot,
            BuyerNotes = v.BuyerNotes,
            AgentNotes = v.AgentNotes,
            Status = v.Status,
            StatusName = v.Status switch
            {
                1 => "Requested",
                2 => "Confirmed",
                3 => "Completed",
                4 => "Cancelled",
                5 => "Rejected",
                _ => $"Status #{v.Status}"
            },
            CreatedAt = v.CreatedAt
        };
    }
}
