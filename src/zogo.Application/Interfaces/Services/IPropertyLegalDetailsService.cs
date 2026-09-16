using zogo.Application.DTOs.Property.LegalDetails;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyLegalDetailsService
{
    Task<GetPropertyLegalDetailsResponse> CreateAsync(
        Guid userId,
        Guid propertyId,
        CreatePropertyLegalDetailsRequest request,
        CancellationToken cancellationToken = default);

    Task<GetPropertyLegalDetailsResponse?> GetByPropertyIdAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<GetPropertyLegalDetailsResponse> UpdateAsync(
        Guid userId,
        Guid propertyId,
        UpdatePropertyLegalDetailsRequest request,
        CancellationToken cancellationToken = default);
}