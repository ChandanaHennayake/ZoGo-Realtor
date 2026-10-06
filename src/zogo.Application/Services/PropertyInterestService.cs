using zogo.Application.DTOs.Buyer;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Enums;

namespace zogo.Application.Services;

public sealed class PropertyInterestService : IPropertyInterestService
{
    private readonly IPropertyInterestRepository _propertyInterestRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyInterestService(
        IPropertyInterestRepository propertyInterestRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyInterestRepository = propertyInterestRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PropertyInterestResponse> SetInterestAsync(
        Guid buyerId,
        Guid propertyId,
        short status,
        CancellationToken cancellationToken = default)
    {
        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        if (!Enum.IsDefined(typeof(PropertyInterestStatus), status))
        {
            throw new ArgumentException(
                $"Invalid property interest status: {status}. Allowed values are 1 (INTERESTED), 2 (CONSIDERING), 3 (NOT_INTERESTED).",
                nameof(status));
        }

        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null)
            throw new KeyNotFoundException("Property not found.");

        if (property.Status != 2)
            throw new InvalidOperationException("Property is not available for buyer viewing.");

        var existing = await _propertyInterestRepository.GetAsync(buyerId, propertyId, cancellationToken);
        if (existing is not null)
        {
            existing.UpdateStatus(status);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MapToResponse(existing);
        }

        var interest = PropertyInterest.Create(propertyId, buyerId, status);
        await _propertyInterestRepository.AddAsync(interest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(interest);
    }

    public async Task<PropertyInterestResponse?> GetInterestAsync(
        Guid buyerId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        if (buyerId == Guid.Empty)
            throw new ArgumentException("Buyer ID is required.", nameof(buyerId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        var interest = await _propertyInterestRepository.GetAsync(buyerId, propertyId, cancellationToken);
        if (interest is null)
            return null;

        return MapToResponse(interest);
    }

    private static PropertyInterestResponse MapToResponse(PropertyInterest interest)
    {
        return new PropertyInterestResponse
        {
            PropertyId = interest.PropertyId,
            Status = interest.Status,
            StatusName = ResolveStatusName(interest.Status),
            CreatedAt = interest.CreatedAt,
            UpdatedAt = interest.UpdatedAt
        };
    }

    private static string ResolveStatusName(short status) =>
        (PropertyInterestStatus)status switch
        {
            PropertyInterestStatus.Interested => "INTERESTED",
            PropertyInterestStatus.Considering => "CONSIDERING",
            PropertyInterestStatus.NotInterested => "NOT_INTERESTED",
            _ => "UNKNOWN"
        };
}
