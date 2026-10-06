using zogo.Application.DTOs.Buyer;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Enums;

namespace zogo.Application.Services;

public sealed class BuyerPropertyInteractionService : IBuyerPropertyInteractionService
{
    private readonly IBuyerPropertyInteractionRepository _interactionRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BuyerPropertyInteractionService(
        IBuyerPropertyInteractionRepository interactionRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _interactionRepository = interactionRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BuyerPropertyInteractionResponse> SetInteractionStatusAsync(
        Guid buyerId,
        Guid propertyId,
        short currentStatus,
        CancellationToken cancellationToken = default)
    {
        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        if (!Enum.IsDefined(typeof(BuyerPropertyInteractionStatus), currentStatus))
        {
            throw new ArgumentException(
                $"Invalid buyer property interaction status: {currentStatus}. Allowed values are 1 to 9.",
                nameof(currentStatus));
        }

        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null)
            throw new KeyNotFoundException("Property not found.");

        if (property.Status != 2)
            throw new InvalidOperationException("Property is not available for buyer viewing.");

        var existing = await _interactionRepository.GetAsync(buyerId, propertyId, cancellationToken);
        if (existing is not null)
        {
            existing.UpdateCurrentStatus(currentStatus);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MapToResponse(existing);
        }

        var interaction = BuyerPropertyInteraction.Create(propertyId, buyerId, currentStatus);
        await _interactionRepository.AddAsync(interaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(interaction);
    }

    public async Task<BuyerPropertyInteractionResponse?> GetInteractionAsync(
        Guid buyerId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        var interaction = await _interactionRepository.GetAsync(buyerId, propertyId, cancellationToken);
        if (interaction is null)
            return null;

        return MapToResponse(interaction);
    }

    private static BuyerPropertyInteractionResponse MapToResponse(BuyerPropertyInteraction interaction)
    {
        return new BuyerPropertyInteractionResponse
        {
            Id = interaction.Id,
            PropertyId = interaction.PropertyId,
            BuyerId = interaction.BuyerId,
            CurrentStatus = interaction.CurrentStatus,
            CurrentStatusName = ResolveStatusName(interaction.CurrentStatus)
        };
    }

    private static string ResolveStatusName(short status) =>
        (BuyerPropertyInteractionStatus)status switch
        {
            BuyerPropertyInteractionStatus.Interested => "INTERESTED",
            BuyerPropertyInteractionStatus.VisitRequested => "VISIT_REQUESTED",
            BuyerPropertyInteractionStatus.Visited => "VISITED",
            BuyerPropertyInteractionStatus.DocumentsRequested => "DOCUMENTS_REQUESTED",
            BuyerPropertyInteractionStatus.Negotiating => "NEGOTIATING",
            BuyerPropertyInteractionStatus.OfferSubmitted => "OFFER_SUBMITTED",
            BuyerPropertyInteractionStatus.OfferAccepted => "OFFER_ACCEPTED",
            BuyerPropertyInteractionStatus.OfferRejected => "OFFER_REJECTED",
            BuyerPropertyInteractionStatus.NotInterested => "NOT_INTERESTED",
            _ => "UNKNOWN"
        };
}
