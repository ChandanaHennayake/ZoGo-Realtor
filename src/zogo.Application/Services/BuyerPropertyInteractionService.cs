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

        // STEP 11: Buyer must NOT mark visited (3)
        if (currentStatus == (short)BuyerPropertyInteractionStatus.Visited)
        {
            throw new InvalidOperationException(
                "Buyers cannot mark properties as visited. Only an assigned agent can mark a visit as completed.");
        }

        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null)
            throw new KeyNotFoundException("Property not found.");

        if (property.Status != 2)
            throw new InvalidOperationException("Property is not available for buyer viewing.");

        var existing = await _interactionRepository.GetAsync(buyerId, propertyId, cancellationToken);

        if (existing is null)
        {
            // Initial interaction must be 1 (INTERESTED)
            if (currentStatus != (short)BuyerPropertyInteractionStatus.Interested)
            {
                throw new InvalidOperationException(
                    $"Initial interaction status must be 1 (INTERESTED). Cannot set status to {currentStatus}.");
            }

            var interaction = BuyerPropertyInteraction.Create(propertyId, buyerId, currentStatus);
            await _interactionRepository.AddAsync(interaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToResponse(interaction);
        }

        // Idempotent update
        if (existing.CurrentStatus == currentStatus)
        {
            return MapToResponse(existing);
        }

        // STEP 8: Buyer visit request (1 -> 2)
        if (currentStatus == (short)BuyerPropertyInteractionStatus.VisitRequested)
        {
            if (existing.CurrentStatus != (short)BuyerPropertyInteractionStatus.Interested)
            {
                throw new InvalidOperationException(
                    $"Cannot request a visit. Current status must be 1 (INTERESTED), but is {existing.CurrentStatus}.");
            }
        }
        // STEP 12 & 15: Document request (3 -> 4)
        else if (currentStatus == (short)BuyerPropertyInteractionStatus.DocumentsRequested)
        {
            if (existing.CurrentStatus != (short)BuyerPropertyInteractionStatus.Visited)
            {
                throw new InvalidOperationException(
                    $"Documents can only be requested after the property has been visited (current status must be 3 - VISITED), but is {existing.CurrentStatus}.");
            }
        }
        else if (currentStatus == (short)BuyerPropertyInteractionStatus.NotInterested)
        {
            // Allowed to mark not interested from any active status
        }
        else if (currentStatus == (short)BuyerPropertyInteractionStatus.Interested)
        {
            // Can reset to interested
        }
        else
        {
            throw new InvalidOperationException(
                $"Invalid status transition from {existing.CurrentStatus} to {currentStatus}.");
        }

        existing.UpdateCurrentStatus(currentStatus);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(existing);
    }

    public async Task<BuyerPropertyInteractionResponse> RequestDocumentsAsync(
        Guid buyerId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null)
            throw new KeyNotFoundException("Property not found.");

        if (property.Status != 2)
            throw new InvalidOperationException("Property is not available for buyer viewing.");

        var existing = await _interactionRepository.GetAsync(buyerId, propertyId, cancellationToken);
        if (existing is null)
        {
            throw new InvalidOperationException(
                "Documents can only be requested after the property has been visited (current status must be 3 - VISITED). No interaction exists.");
        }

        if (existing.CurrentStatus != (short)BuyerPropertyInteractionStatus.Visited)
        {
            throw new InvalidOperationException(
                $"Documents can only be requested after the property has been visited (current status must be 3 - VISITED), but current status is {existing.CurrentStatus}.");
        }

        existing.UpdateCurrentStatus((short)BuyerPropertyInteractionStatus.DocumentsRequested);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(existing);
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
