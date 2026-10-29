namespace zogo.Application.Interfaces.Repositories;

public interface ISystemConfigurationRepository
{
    Task<string> PeekNextPropertyReferenceNoAsync(CancellationToken cancellationToken = default);
    Task<string> GenerateAndIncrementPropertyReferenceNoAsync(CancellationToken cancellationToken = default);
    Task<string> GetNextPropertyReferenceNoAsync(CancellationToken cancellationToken = default);
}
