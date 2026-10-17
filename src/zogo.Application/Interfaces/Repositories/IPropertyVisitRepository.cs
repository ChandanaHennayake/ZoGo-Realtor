using zogo.Application.DTOs.Visits;
using zogo.Domain.Entities.Buyer;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyVisitRepository
{
    Task AddAsync(PropertyVisit visit, CancellationToken cancellationToken = default);

    Task<PropertyVisit?> GetByIdAsync(Guid visitId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitResponse>> GetVisitsByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitResponse>> GetVisitsByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitResponse>> GetVisitsByAgentDistrictsAsync(IReadOnlyList<short> districtIds, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitResponse>> GetAllVisitsAsync(short? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
