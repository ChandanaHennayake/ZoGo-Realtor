using zogo.Application.DTOs.Properties;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<CreatePropertyResponse> CreateDraftAsync(
        Guid userId,
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default);



    Task<GetPropertyResponse?> GetByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);


    Task<IReadOnlyList<GetPropertyResponse>> GetMyPropertiesAsync(
    Guid userId,
    CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
    Guid userId,
    Guid propertyId,
    UpdatePropertyRequest request,
    CancellationToken cancellationToken = default);
}