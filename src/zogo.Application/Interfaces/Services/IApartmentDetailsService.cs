using zogo.Application.DTOs.Properties;

namespace zogo.Application.Interfaces.Services;

public interface IApartmentDetailsService
{
    Task<GetApartmentDetailsResponse> CreateAsync(
        Guid userId,
        Guid propertyId,
        CreateApartmentDetailsRequest request,
        CancellationToken cancellationToken = default);

    Task<GetApartmentDetailsResponse?> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid userId,
        Guid propertyId,
        UpdateApartmentDetailsRequest request,
        CancellationToken cancellationToken = default);
}