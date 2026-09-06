using zogo.Application.DTOs.Properties;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<CreatePropertyResponse> CreateDraftAsync(
        Guid userId,
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default);
}