using zogo.Domain.Entities.Identity;

namespace zogo.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}