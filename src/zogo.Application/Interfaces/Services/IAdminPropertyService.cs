using zogo.Application.DTOs.Admin;

namespace zogo.Application.Interfaces.Services;

public interface IAdminPropertyService
{
    Task<IReadOnlyList<AdminPropertySummaryResponse>> GetPropertiesByStatusAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<AdminPropertyReviewDetailsResponse?> GetPropertyReviewDetailsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<bool> StartReviewAsync(
        Guid propertyId,
        Guid adminUserId,
        CancellationToken cancellationToken = default);

    Task<bool> ApprovePropertyAsync(
        Guid propertyId,
        Guid adminUserId,
        CancellationToken cancellationToken = default);

    Task<bool> RequestChangesAsync(
        Guid propertyId,
        RequestPropertyChangesRequest request,
        Guid adminUserId,
        CancellationToken cancellationToken = default);

    Task<bool> RejectPropertyAsync(
        Guid propertyId,
        RejectPropertyRequest request,
        Guid adminUserId,
        CancellationToken cancellationToken = default);

    Task<AdminDashboardStatsResponse> GetDashboardStatsAsync(
        CancellationToken cancellationToken = default);
}
