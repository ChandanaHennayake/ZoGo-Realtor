using zogo.Application.DTOs.Visits;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyVisitService
{
    Task<VisitResponse> ScheduleVisitAsync(
        Guid buyerId,
        CreateVisitRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitResponse>> GetMyVisitsAsync(
        Guid buyerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitResponse>> GetPropertyVisitsAsync(
        Guid propertyId,
        Guid requestingUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitResponse>> GetAgentVisitsAsync(
        Guid agentUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitResponse>> GetAllVisitsAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<VisitResponse> ConfirmVisitAsync(
        Guid visitId,
        ConfirmVisitRequest request,
        Guid confirmedByUserId,
        CancellationToken cancellationToken = default);

    Task<VisitResponse> CompleteVisitAsync(
        Guid visitId,
        CompleteVisitRequest request,
        Guid completedByUserId,
        CancellationToken cancellationToken = default);

    Task<VisitResponse> CancelVisitAsync(
        Guid visitId,
        CancelVisitRequest request,
        Guid cancelledByUserId,
        CancellationToken cancellationToken = default);
}
