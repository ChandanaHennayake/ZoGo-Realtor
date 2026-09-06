using zogo.Application.Constants;
using zogo.Application.DTOs.Seller;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Identity;

namespace zogo.Application.Services;

public sealed class SellerService : ISellerService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SellerService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SellerActivationResponse> ActivateSellerAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // 1. Find Seller role
        var sellerRole =
            await _roleRepository.GetByCodeAsync(
                RoleCodes.Seller,
                cancellationToken);

        if (sellerRole is null)
        {
            throw new ApplicationException(
                "Seller role was not found.");
        }

        if (!sellerRole.IsActive)
        {
            throw new ApplicationException(
                "Seller role is not active.");
        }

        // 2. Get user
        // We need a repository method that gets the user by ID.
        var user =
            await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

        if (user is null)
        {
            throw new ApplicationException(
                "User was not found.");
        }

        // 3. Check if already Seller
        var alreadySeller =
            user.UserRoles.Any(
                x => x.RoleId == sellerRole.Id);

        if (alreadySeller)
        {
            return new SellerActivationResponse
            {
                UserId = user.Id,
                SellerActivated = true,
                Message = "Seller account is already active."
            };
        }

        // 4. Create Seller role
        var userRole = UserRole.Create(
            user.Id,
            sellerRole.Id,
            null);

        // 5. Add role to user
        user.AddRole(userRole);

        // 6. Save
        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        // 7. Return
        return new SellerActivationResponse
        {
            UserId = user.Id,
            SellerActivated = true,
            Message = "Seller account activated successfully."
        };
    }
}