using zogo.Application.DTOs.Authentication;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Identity;

namespace zogo.Application.Services.Authentication;

public sealed class AuthenticationService : IAuthenticationService
{
    private const string DefaultRoleCode = "BYR";

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGoogleAuthenticationService _googleAuthenticationService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthenticationService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork,
        IGoogleAuthenticationService googleAuthenticationService,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
        _googleAuthenticationService = googleAuthenticationService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        // 1. Check existing user
        var existingUser =
            await _userRepository.GetByEmailAsync(
                email,
                cancellationToken);

        if (existingUser is not null)
        {
            throw new ApplicationException(
                "A user with this email already exists.");
        }

        // 2. Get default BUYER role
        var buyerRole =
            await _roleRepository.GetByCodeAsync(
                DefaultRoleCode,
                cancellationToken);

        if (buyerRole is null)
        {
            throw new ApplicationException(
                "Default BUYER role was not found.");
        }

        // 3. Hash password
        var passwordHash =
            _passwordService.HashPassword(
                request.Password);

        // 4. Create user
        var user = User.Create(
            request.FirstName,
            request.LastName,
            email,
            request.PhoneNumber);

        // 5. Create LOCAL authentication provider
        var authenticationProvider =
            UserAuthenticationProvider.CreateLocal(
                user.Id,
                email,
                passwordHash);

        user.AddAuthenticationProvider(
            authenticationProvider);

        // 6. Assign BUYER role
        var userRole = UserRole.Create(
            user.Id,
            buyerRole.Id,
            null);

        user.AddRole(userRole);

        // 7. Add user
        await _userRepository.AddAsync(
            user,
            cancellationToken);

        // 8. Save
        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        // 9. Return registration response
        return new RegisterResponse
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }



    public async Task<AuthenticationResponse> LoginAsync(
    LoginRequest request,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email is required.",
                nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(
                "Password is required.",
                nameof(request));
        }

        // 1. Find user
        var user =
            await _userRepository.GetByEmailAsync(
                email,
                cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        // 2. Check user status
        if (user.DeletedAt is not null)
        {
            throw new UnauthorizedAccessException(
                "This account is no longer available.");
        }

        // 3. Get LOCAL authentication provider
        var localProvider =
            await _userRepository.GetLocalProviderAsync(
                user.Id,
                cancellationToken);

        if (localProvider is null)
        {
            throw new UnauthorizedAccessException(
                "This account does not have local password authentication.");
        }

        // 4. Check password hash
        if (string.IsNullOrWhiteSpace(localProvider.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        // 5. Verify password
        var passwordValid =
            _passwordService.VerifyPassword(
                request.Password,
                localProvider.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        // 6. Get user roles
        var roles = user.UserRoles
            .Where(x => x.Role is not null)
            .Select(x => x.Role.Code)
            .ToList();

        // 7. Generate access token
        var accessToken =
            _jwtTokenService.GenerateAccessToken(
                user.Id,
                user.Email,
                roles);

        var accessTokenExpiresAt =
            _jwtTokenService.GetAccessTokenExpiration();

        // 8. Generate refresh token
        var refreshToken =
            _jwtTokenService.GenerateRefreshToken();

        // 9. Hash refresh token before storing
        var refreshTokenHash =
            _jwtTokenService.HashRefreshToken(
                refreshToken);

        // 10. Create refresh token entity
        var refreshTokenEntity =
            RefreshToken.Create(
                user.Id,
                refreshTokenHash,
                DateTimeOffset.UtcNow.AddDays(30));

        // 11. Save refresh token
        await _refreshTokenRepository.AddAsync(
            refreshTokenEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        // 12. Return response
        return new AuthenticationResponse
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            Roles = roles
        };
    }


    public async Task<AuthenticationResponse> GoogleLoginAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.IdToken))
        {
            throw new ArgumentException(
                "Google ID token is required.",
                nameof(request));
        }

        // =========================================================
        // 1. Validate Google ID token
        // =========================================================

        var googleUser =
            await _googleAuthenticationService.ValidateTokenAsync(
                request.IdToken,
                cancellationToken);

        if (googleUser is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid Google ID token.");
        }

        if (!googleUser.EmailVerified)
        {
            throw new UnauthorizedAccessException(
                "Google email is not verified.");
        }

        var email = googleUser.Email
            .Trim()
            .ToLowerInvariant();

        // =========================================================
        // 2. Find existing ZoGo user
        // =========================================================

        var user =
            await _userRepository.GetByEmailAsync(
                email,
                cancellationToken);

        // =========================================================
        // 3. If user doesn't exist, register Google user
        // =========================================================

        if (user is null)
        {
            // Get default BUYER role
            var buyerRole =
                await _roleRepository.GetByCodeAsync(
                    DefaultRoleCode,
                    cancellationToken);

            if (buyerRole is null)
            {
                throw new ApplicationException(
                    "Default BUYER role was not found.");
            }

            // Create user
            user = User.Create(
                googleUser.FirstName,
                googleUser.LastName,
                email,
                null);

            // Create GOOGLE authentication provider
            var googleProvider =
                UserAuthenticationProvider.CreateGoogle(
                    user.Id,
                    googleUser.Subject,
                    email);

            user.AddAuthenticationProvider(
                googleProvider);

            // Assign BUYER role
            var userRole = UserRole.Create(
                user.Id,
                buyerRole.Id,
                null);

            user.AddRole(userRole);

            // Save new user
            await _userRepository.AddAsync(
                user,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        // =========================================================
        // 4. Get roles
        // =========================================================

        var roles = user.UserRoles
            .Where(x => x.Role is not null)
            .Select(x => x.Role.Code)
            .ToList();

        // =========================================================
        // 5. Generate JWT access token
        // =========================================================

        var accessToken =
            _jwtTokenService.GenerateAccessToken(
                user.Id,
                user.Email,
                roles);

        var accessTokenExpiresAt =
            _jwtTokenService.GetAccessTokenExpiration();

        // =========================================================
        // 6. Generate refresh token
        // =========================================================

        var refreshToken =
            _jwtTokenService.GenerateRefreshToken();

        var refreshTokenHash =
            _jwtTokenService.HashRefreshToken(
                refreshToken);

        // =========================================================
        // 7. Save refresh token
        // =========================================================

        var refreshTokenEntity =
            RefreshToken.Create(
                user.Id,
                refreshTokenHash,
                DateTimeOffset.UtcNow.AddDays(30));

        await _refreshTokenRepository.AddAsync(
            refreshTokenEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        // =========================================================
        // 8. Return authentication response
        // =========================================================

        return new AuthenticationResponse
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            Roles = roles
        };
    }
}