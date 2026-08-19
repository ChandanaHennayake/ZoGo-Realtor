using zogo.Domain.Entities.Identity;

namespace zogo.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);


    Task<UserAuthenticationProvider?> GetLocalProviderAsync(
     Guid userId,
     CancellationToken cancellationToken = default);


    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);
}