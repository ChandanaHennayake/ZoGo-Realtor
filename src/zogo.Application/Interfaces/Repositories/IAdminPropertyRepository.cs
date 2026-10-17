using zogo.Application.DTOs.Admin;
using zogo.Domain.Entities.Master;

namespace zogo.Application.Interfaces.Repositories;

public interface IAdminPropertyRepository
{
    Task<IReadOnlyList<AdminPropertySummaryResponse>> GetPropertiesByStatusAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<AdminPropertyReviewDetailsResponse?> GetPropertyReviewDetailsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<Property?> GetByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<AdminDashboardStatsResponse> GetDashboardStatsAsync(
        CancellationToken cancellationToken = default);
}
