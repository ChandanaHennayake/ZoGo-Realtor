using zogo.Application.DTOs.Property.Financials;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyFinancialsService
{
    Task<GetPropertyFinancialsResponse> CreateAsync(
        Guid userId,
        Guid propertyId,
        CreatePropertyFinancialsRequest request,
        CancellationToken cancellationToken = default);

    Task<GetPropertyFinancialsResponse?> GetByPropertyIdAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<GetPropertyFinancialsResponse> UpdateAsync(
        Guid userId,
        Guid propertyId,
        UpdatePropertyFinancialsRequest request,
        CancellationToken cancellationToken = default);
}