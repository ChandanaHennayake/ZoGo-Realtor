using zogo.Application.DTOs.Offers;
using zogo.Domain.Entities.Buyer;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyOfferRepository
{
    Task<PropertyOffer?> GetByIdAsync(Guid offerId, CancellationToken cancellationToken = default);
    Task<OfferResponse?> GetOfferDetailsAsync(Guid offerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OfferResponse>> GetOffersByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OfferResponse>> GetOffersByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OfferResponse>> GetAllOffersAsync(short? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(PropertyOffer offer, CancellationToken cancellationToken = default);
    Task UpdateAsync(PropertyOffer offer, CancellationToken cancellationToken = default);
}
