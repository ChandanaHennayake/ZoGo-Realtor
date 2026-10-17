using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Agent;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/admin/agents")]
[Authorize(Roles = "ADM,Admin")]
public sealed class AdminAgentController : ControllerBase
{
    private readonly IAdminAgentService _adminAgentService;

    public AdminAgentController(IAdminAgentService adminAgentService)
    {
        _adminAgentService = adminAgentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAgent(
        [FromBody] CreateAgentRequest request,
        CancellationToken cancellationToken)
    {
        var adminUserId = GetUserId();

        try
        {
            var result = await _adminAgentService.CreateAgentAsync(request, adminUserId, cancellationToken);
            return CreatedAtAction(nameof(GetAgentById), new { agentId = result.AgentId }, ApiResponse<AgentResponse>.Ok(result, "Agent created successfully."));
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Values.SelectMany(x => x).ToList();
            return BadRequest(ApiResponse.Fail(ex.Message, errors));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (zogo.Application.Exceptions.ApplicationException ex)
        {
            return Conflict(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAgents(CancellationToken cancellationToken)
    {
        var result = await _adminAgentService.GetAllAgentsAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AgentResponse>>.Ok(result));
    }

    [HttpGet("{agentId:guid}")]
    public async Task<IActionResult> GetAgentById(Guid agentId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _adminAgentService.GetAgentByIdAsync(agentId, cancellationToken);
            return Ok(ApiResponse<AgentResponse>.Ok(result));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{agentId:guid}")]
    public async Task<IActionResult> UpdateAgent(
        Guid agentId,
        [FromBody] UpdateAgentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _adminAgentService.UpdateAgentAsync(agentId, request, cancellationToken);
            return Ok(ApiResponse<AgentResponse>.Ok(result, "Agent updated successfully."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Values.SelectMany(x => x).ToList();
            return BadRequest(ApiResponse.Fail(ex.Message, errors));
        }
    }

    [HttpPut("{agentId:guid}/districts")]
    public async Task<IActionResult> AssignDistricts(
        Guid agentId,
        [FromBody] AssignAgentDistrictsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _adminAgentService.AssignDistrictsAsync(agentId, request, cancellationToken);
            return Ok(ApiResponse<AgentResponse>.Ok(result, "Districts updated successfully."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Values.SelectMany(x => x).ToList();
            return BadRequest(ApiResponse.Fail(ex.Message, errors));
        }
    }

    [HttpPut("{agentId:guid}/activate")]
    public async Task<IActionResult> ActivateAgent(Guid agentId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _adminAgentService.ActivateAgentAsync(agentId, cancellationToken);
            return Ok(ApiResponse<AgentResponse>.Ok(result, "Agent activated successfully."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{agentId:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAgent(Guid agentId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _adminAgentService.DeactivateAgentAsync(agentId, cancellationToken);
            return Ok(ApiResponse<AgentResponse>.Ok(result, "Agent deactivated successfully."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : null;
    }
}
