using zogo.Application.DTOs.Offers;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Entities.Notification;
using zogo.Domain.Enums;

namespace zogo.Application.Services;

public sealed class PropertyOfferService : IPropertyOfferService
{
    private readonly IPropertyOfferRepository _offerRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IBuyerPropertyInteractionRepository _interactionRepository;
    private readonly IUserNotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyOfferService(
        IPropertyOfferRepository offerRepository,
        IPropertyRepository propertyRepository,
        IBuyerPropertyInteractionRepository interactionRepository,
        IUserNotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _offerRepository = offerRepository;
        _propertyRepository = propertyRepository;
        _interactionRepository = interactionRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OfferResponse> SubmitOfferAsync(
        Guid buyerId,
        CreateOfferRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.OfferAmount <= 0)
            throw new ArgumentException("Offer amount must be greater than zero.", nameof(request.OfferAmount));

        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property == null || property.DeletedAt.HasValue)
            throw new NotFoundException("Property", request.PropertyId);

        if (property.CreatedBy == buyerId)
            throw new InvalidOperationException("You cannot submit an offer on your own property listing.");

        var offer = PropertyOffer.Create(
            request.PropertyId,
            buyerId,
            request.OfferAmount,
            request.BuyerNotes,
            request.ExpiresAt);

        await _offerRepository.AddAsync(offer, cancellationToken);

        // Update or create buyer interaction status to OfferSubmitted (6)
        var interaction = await _interactionRepository.GetAsync(buyerId, request.PropertyId, cancellationToken);
        if (interaction != null)
        {
            interaction.UpdateCurrentStatus((short)BuyerPropertyInteractionStatus.OfferSubmitted);
        }
        else
        {
            var newInteraction = BuyerPropertyInteraction.Create(
                request.PropertyId,
                buyerId,
                (short)BuyerPropertyInteractionStatus.OfferSubmitted);
            await _interactionRepository.AddAsync(newInteraction, cancellationToken);
        }

        var offerNotif = UserNotification.Create(
            property.OwnerUserId,
            "OFFER_RECEIVED",
            "New Purchase Offer Received! 💼",
            $"A buyer submitted an offer of LKR {request.OfferAmount:N0} for '{property.Title}'.",
            offer.Id.ToString(),
            "Offer");
        await _notificationRepository.AddAsync(offerNotif, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _offerRepository.GetOfferDetailsAsync(offer.Id, cancellationToken);
        return response!;
    }

    public async Task<OfferResponse> CounterOfferAsync(
        Guid offerId,
        CounterOfferRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var offer = await _offerRepository.GetByIdAsync(offerId, cancellationToken);
        if (offer == null || offer.DeletedAt.HasValue)
            throw new NotFoundException("PropertyOffer", offerId);

        offer.Counter(request.CounterAmount, request.SellerNotes);
        await _offerRepository.UpdateAsync(offer, cancellationToken);

        // Update interaction to Negotiating (5)
        var interaction = await _interactionRepository.GetAsync(offer.BuyerId, offer.PropertyId, cancellationToken);
        if (interaction != null)
        {
            interaction.UpdateCurrentStatus((short)BuyerPropertyInteractionStatus.Negotiating);
        }

        var counterNotif = UserNotification.Create(
            offer.BuyerId,
            "OFFER_COUNTERED",
            "Counteroffer Received",
            $"The seller countered your offer on listing to LKR {request.CounterAmount:N0}.",
            offer.Id.ToString(),
            "Offer");
        await _notificationRepository.AddAsync(counterNotif, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _offerRepository.GetOfferDetailsAsync(offer.Id, cancellationToken);
        return response!;
    }

    public async Task<OfferResponse> AcceptOfferAsync(
        Guid offerId,
        AcceptOfferRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var offer = await _offerRepository.GetByIdAsync(offerId, cancellationToken);
        if (offer == null || offer.DeletedAt.HasValue)
            throw new NotFoundException("PropertyOffer", offerId);

        offer.Accept(request?.SellerNotes);
        await _offerRepository.UpdateAsync(offer, cancellationToken);

        // Update interaction to OfferAccepted (7)
        var interaction = await _interactionRepository.GetAsync(offer.BuyerId, offer.PropertyId, cancellationToken);
        if (interaction != null)
        {
            interaction.UpdateCurrentStatus((short)BuyerPropertyInteractionStatus.OfferAccepted);
        }

        var acceptNotif = UserNotification.Create(
            offer.BuyerId,
            "OFFER_ACCEPTED",
            "Offer Accepted! 🎉",
            $"Congratulations! Your offer of LKR {offer.OfferAmount:N0} has been accepted. Proceed to closing & deed verification.",
            offer.Id.ToString(),
            "Offer");
        await _notificationRepository.AddAsync(acceptNotif, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _offerRepository.GetOfferDetailsAsync(offer.Id, cancellationToken);
        return response!;
    }

    public async Task<OfferResponse> RejectOfferAsync(
        Guid offerId,
        RejectOfferRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var offer = await _offerRepository.GetByIdAsync(offerId, cancellationToken);
        if (offer == null || offer.DeletedAt.HasValue)
            throw new NotFoundException("PropertyOffer", offerId);

        offer.Reject(request?.Reason);
        await _offerRepository.UpdateAsync(offer, cancellationToken);

        // Update interaction to OfferRejected (8)
        var interaction = await _interactionRepository.GetAsync(offer.BuyerId, offer.PropertyId, cancellationToken);
        if (interaction != null)
        {
            interaction.UpdateCurrentStatus((short)BuyerPropertyInteractionStatus.OfferRejected);
        }

        var rejectNotif = UserNotification.Create(
            offer.BuyerId,
            "OFFER_REJECTED",
            "Offer Declined",
            $"Your offer was declined by the seller.",
            offer.Id.ToString(),
            "Offer");
        await _notificationRepository.AddAsync(rejectNotif, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _offerRepository.GetOfferDetailsAsync(offer.Id, cancellationToken);
        return response!;
    }

    public async Task<OfferResponse> WithdrawOfferAsync(
        Guid offerId,
        WithdrawOfferRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var offer = await _offerRepository.GetByIdAsync(offerId, cancellationToken);
        if (offer == null || offer.DeletedAt.HasValue)
            throw new NotFoundException("PropertyOffer", offerId);

        if (offer.BuyerId != userId)
            throw new InvalidOperationException("Only the buyer who submitted the offer can withdraw it.");

        offer.Withdraw(request?.Reason);
        await _offerRepository.UpdateAsync(offer, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _offerRepository.GetOfferDetailsAsync(offer.Id, cancellationToken);
        return response!;
    }

    public async Task<IReadOnlyList<OfferResponse>> GetMyOffersAsync(
        Guid buyerId,
        CancellationToken cancellationToken = default)
    {
        return await _offerRepository.GetOffersByBuyerIdAsync(buyerId, cancellationToken);
    }

    public async Task<IReadOnlyList<OfferResponse>> GetPropertyOffersAsync(
        Guid propertyId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _offerRepository.GetOffersByPropertyIdAsync(propertyId, cancellationToken);
    }

    public async Task<IReadOnlyList<OfferResponse>> GetAllOffersAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 50 : pageSize;
        return await _offerRepository.GetAllOffersAsync(status, page, size, cancellationToken);
    }
}
