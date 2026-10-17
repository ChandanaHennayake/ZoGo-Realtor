using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Agent;
using zogo.Application.DTOs.Buyer;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/agent")]
[Authorize(Roles = "AGT")]
public sealed class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _agentService.GetDashboardAsync(userId.Value, cancellationToken);
            return Ok(ApiResponse<AgentDashboardResponse>.Ok(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse.Fail(ex.Message));
        }
    }

    [HttpGet("properties/interested")]
    public async Task<IActionResult> GetInterestedProperties(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _agentService.GetInterestedPropertiesAsync(userId.Value, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<AgentInterestedPropertyResponse>>.Ok(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse.Fail(ex.Message));
        }
    }

    [HttpGet("visits/pending")]
    public async Task<IActionResult> GetPendingVisits(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _agentService.GetPendingVisitsAsync(userId.Value, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<AgentPendingVisitResponse>>.Ok(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("properties/{propertyId:guid}/visit")]
    public async Task<IActionResult> MarkVisited(
        Guid propertyId,
        [FromBody] MarkVisitedRequest? request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _agentService.MarkVisitedAsync(
                userId.Value,
                propertyId,
                request?.BuyerId,
                cancellationToken);

            return Ok(ApiResponse<BuyerPropertyInteractionResponse>.Ok(result, "Visit marked as completed."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse.Fail(ex.Message));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpGet("documents/pending")]
    public async Task<IActionResult> GetPendingDocuments(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _agentService.GetPendingDocumentsAsync(userId.Value, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<AgentPendingDocumentResponse>>.Ok(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse.Fail(ex.Message));
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
