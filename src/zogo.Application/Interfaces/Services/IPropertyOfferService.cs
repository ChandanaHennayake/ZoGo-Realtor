using zogo.Application.DTOs.Offers;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyOfferService
{
    Task<OfferResponse> SubmitOfferAsync(Guid buyerId, CreateOfferRequest request, CancellationToken cancellationToken = default);
    Task<OfferResponse> CounterOfferAsync(Guid offerId, CounterOfferRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<OfferResponse> AcceptOfferAsync(Guid offerId, AcceptOfferRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<OfferResponse> RejectOfferAsync(Guid offerId, RejectOfferRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<OfferResponse> WithdrawOfferAsync(Guid offerId, WithdrawOfferRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OfferResponse>> GetMyOffersAsync(Guid buyerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OfferResponse>> GetPropertyOffersAsync(Guid propertyId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OfferResponse>> GetAllOffersAsync(short? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
