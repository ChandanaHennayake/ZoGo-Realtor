using zogo.Application.DTOs.Property.Financials;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Property;

namespace zogo.Application.Services;

public sealed class PropertyFinancialsService : IPropertyFinancialsService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyFinancialsRepository _propertyFinancialsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyFinancialsService(
        IPropertyRepository propertyRepository,
        IPropertyFinancialsRepository propertyFinancialsRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _propertyFinancialsRepository = propertyFinancialsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetPropertyFinancialsResponse> CreateAsync(
        Guid userId,
        Guid propertyId,
        CreatePropertyFinancialsRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            throw new KeyNotFoundException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You are not the owner of this property.");

        if (await _propertyFinancialsRepository.ExistsAsync(
                propertyId,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Financial information already exists for this property.");
        }

        var financials = PropertyFinancials.Create(
            propertyId,
            request.MaintenanceFee,
            request.MaintenanceFeePeriod,
            request.SinkingFundAmount,
            request.SinkingFundPeriod,
            request.BillsUpToDate,
            request.HasOutstandingCharges,
            request.OutstandingAmount,
            request.OutstandingDescription);

        await _propertyFinancialsRepository.AddAsync(
            financials,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(financials);
    }

    public async Task<GetPropertyFinancialsResponse?> GetByPropertyIdAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            return null;

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You are not the owner of this property.");

        var financials = await _propertyFinancialsRepository
            .GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        return financials is null
            ? null
            : MapToResponse(financials);
    }

    public async Task<GetPropertyFinancialsResponse> UpdateAsync(
        Guid userId,
        Guid propertyId,
        UpdatePropertyFinancialsRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null || property.DeletedAt.HasValue)
            throw new KeyNotFoundException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You are not the owner of this property.");

        var financials = await _propertyFinancialsRepository
            .GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        if (financials is null)
            throw new KeyNotFoundException(
                "Financial information not found for this property.");

        financials.Update(
            request.MaintenanceFee,
            request.MaintenanceFeePeriod,
            request.SinkingFundAmount,
            request.SinkingFundPeriod,
            request.BillsUpToDate,
            request.HasOutstandingCharges,
            request.OutstandingAmount,
            request.OutstandingDescription);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(financials);
    }

    private static GetPropertyFinancialsResponse MapToResponse(
        PropertyFinancials financials)
    {
        return new GetPropertyFinancialsResponse
        {
            PropertyId = financials.PropertyId,
            MaintenanceFee = financials.MaintenanceFee,
            MaintenanceFeePeriod = financials.MaintenanceFeePeriod,
            SinkingFundAmount = financials.SinkingFundAmount,
            SinkingFundPeriod = financials.SinkingFundPeriod,
            BillsUpToDate = financials.BillsUpToDate,
            HasOutstandingCharges = financials.HasOutstandingCharges,
            OutstandingAmount = financials.OutstandingAmount,
            OutstandingDescription = financials.OutstandingDescription
        };
    }
}