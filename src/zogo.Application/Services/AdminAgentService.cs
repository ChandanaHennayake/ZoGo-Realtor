using zogo.Application.Constants;
using zogo.Application.DTOs.Agent;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Agent;
using zogo.Domain.Entities.Identity;

namespace zogo.Application.Services;

public sealed class AdminAgentService : IAdminAgentService
{
    private readonly IAgentRepository _agentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;

    public AdminAgentService(
        IAgentRepository agentRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork)
    {
        _agentRepository = agentRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AgentResponse> CreateAgentAsync(
        CreateAgentRequest request,
        Guid? adminUserId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ValidationException("FirstName", "First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ValidationException("LastName", "Last name is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email", "Email is required.");

        if (request.CityId <= 0)
            throw new ValidationException("CityId", "Valid city ID is required.");

        if (request.DistrictIds is null || request.DistrictIds.Count == 0)
            throw new ValidationException("DistrictIds", "At least one district must be assigned.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // 1. Verify city exists
        var cityExists = await _agentRepository.CityExistsAsync(request.CityId, cancellationToken);
        if (!cityExists)
            throw new NotFoundException("City", request.CityId);

        // 2. Verify all districts exist and are active
        var distinctDistrictIds = request.DistrictIds.Distinct().ToList();
        var existingDistrictCount = await _agentRepository.CountActiveDistrictsAsync(distinctDistrictIds, cancellationToken);

        if (existingDistrictCount != distinctDistrictIds.Count)
            throw new ValidationException("DistrictIds", "One or more assigned districts do not exist or are inactive.");

        // 3. Find AGT role
        var agentRole = await _roleRepository.GetByCodeAsync(RoleCodes.Agent, cancellationToken);
        if (agentRole is null)
        {
            throw new zogo.Application.Exceptions.ApplicationException("Agent role (AGT) was not found in the system.");
        }

        // 4. Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        User user;

        if (existingUser is not null)
        {
            var alreadyHasAgentProfile = await _agentRepository.ExistsForUserAsync(existingUser.Id, cancellationToken);
            if (alreadyHasAgentProfile)
                throw new zogo.Application.Exceptions.ApplicationException("An agent profile already exists for this email.");

            user = existingUser;
            user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber);
        }
        else
        {
            user = User.Create(
                request.FirstName,
                request.LastName,
                normalizedEmail,
                request.PhoneNumber);

            // Create default local authentication provider so agent can log in
            var defaultPassword = "Agent@" + Guid.NewGuid().ToString("N")[..6];
            var passwordHash = _passwordService.HashPassword(defaultPassword);
            var authProvider = UserAuthenticationProvider.CreateLocal(user.Id, normalizedEmail, passwordHash);
            user.AddAuthenticationProvider(authProvider);

            await _userRepository.AddAsync(user, cancellationToken);
        }

        // 5. Assign AGT role if not already assigned
        var hasAgtRole = user.UserRoles.Any(ur => ur.RoleId == agentRole.Id);
        if (!hasAgtRole)
        {
            var userRole = UserRole.Create(user.Id, agentRole.Id, adminUserId);
            user.AddRole(userRole);
        }

        // 6. Create Agent profile
        var agentProfile = AgentProfile.Create(user.Id, request.CityId, true);
        await _agentRepository.AddProfileAsync(agentProfile, cancellationToken);

        // 7. Assign districts
        var agentDistricts = distinctDistrictIds
            .Select(districtId => AgentDistrict.Create(agentProfile.Id, districtId))
            .ToList();

        await _agentRepository.AddDistrictsAsync(agentDistricts, cancellationToken);

        // 8. Save transactionally
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 9. Fetch full profile and return
        var created = await _agentRepository.GetByIdAsync(agentProfile.Id, cancellationToken);
        return MapToAgentResponse(created!);
    }

    public async Task<IReadOnlyList<AgentResponse>> GetAllAgentsAsync(CancellationToken cancellationToken = default)
    {
        var agents = await _agentRepository.GetAllAsync(cancellationToken);
        return agents.Select(MapToAgentResponse).ToList();
    }

    public async Task<AgentResponse> GetAgentByIdAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null)
            throw new NotFoundException("Agent", agentId);

        return MapToAgentResponse(agent);
    }

    public async Task<AgentResponse> UpdateAgentAsync(
        Guid agentId,
        UpdateAgentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null)
            throw new NotFoundException("Agent", agentId);

        if (request.CityId > 0 && request.CityId != agent.CityId)
        {
            var cityExists = await _agentRepository.CityExistsAsync(request.CityId, cancellationToken);
            if (!cityExists)
                throw new NotFoundException("City", request.CityId);

            agent.Update(request.CityId, request.IsActive);
        }
        else if (request.IsActive.HasValue)
        {
            agent.Update(agent.CityId, request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
        {
            var firstName = string.IsNullOrWhiteSpace(request.FirstName) ? agent.User.FirstName : request.FirstName;
            var lastName = string.IsNullOrWhiteSpace(request.LastName) ? agent.User.LastName : request.LastName;
            agent.User.UpdateProfile(firstName, lastName, request.PhoneNumber ?? agent.User.PhoneNumber);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        return MapToAgentResponse(updated!);
    }

    public async Task<AgentResponse> AssignDistrictsAsync(
        Guid agentId,
        AssignAgentDistrictsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null)
            throw new NotFoundException("Agent", agentId);

        if (request.DistrictIds is null || request.DistrictIds.Count == 0)
            throw new ValidationException("DistrictIds", "At least one district must be specified.");

        var distinctDistrictIds = request.DistrictIds.Distinct().ToList();
        var existingDistrictCount = await _agentRepository.CountActiveDistrictsAsync(distinctDistrictIds, cancellationToken);

        if (existingDistrictCount != distinctDistrictIds.Count)
            throw new ValidationException("DistrictIds", "One or more assigned districts do not exist or are inactive.");

        await _agentRepository.RemoveDistrictsAsync(agentId, cancellationToken);

        var newDistricts = distinctDistrictIds
            .Select(dId => AgentDistrict.Create(agentId, dId))
            .ToList();

        await _agentRepository.AddDistrictsAsync(newDistricts, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        return MapToAgentResponse(updated!);
    }

    public async Task<AgentResponse> ActivateAgentAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null)
            throw new NotFoundException("Agent", agentId);

        agent.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToAgentResponse(agent);
    }

    public async Task<AgentResponse> DeactivateAgentAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null)
            throw new NotFoundException("Agent", agentId);

        agent.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToAgentResponse(agent);
    }

    private static AgentResponse MapToAgentResponse(AgentProfile profile)
    {
        return new AgentResponse
        {
            AgentId = profile.Id,
            UserId = profile.UserId,
            Name = $"{profile.User.FirstName} {profile.User.LastName}".Trim(),
            FirstName = profile.User.FirstName,
            LastName = profile.User.LastName,
            Email = profile.User.Email,
            PhoneNumber = profile.User.PhoneNumber,
            City = new AgentCityDto
            {
                Id = profile.CityId,
                Name = profile.City?.Name ?? string.Empty
            },
            Districts = profile.AgentDistricts
                .Select(ad => new AgentDistrictDto
                {
                    Id = ad.DistrictId,
                    Name = ad.District?.Name ?? string.Empty
                })
                .ToList(),
            IsActive = profile.IsActive,
            CreatedAt = profile.CreatedAt
        };
    }
}
