using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Visits;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/visits")]
[Authorize]
public sealed class PropertyVisitController : ControllerBase
{
    private readonly IPropertyVisitService _visitService;

    public PropertyVisitController(IPropertyVisitService visitService)
    {
        _visitService = visitService;
    }

    [HttpPost]
    public async Task<IActionResult> ScheduleVisit(
        [FromBody] CreateVisitRequest request,
        CancellationToken cancellationToken)
    {
        var buyerId = GetUserId();
        if (buyerId is null)
            return Unauthorized();

        try
        {
            var result = await _visitService.ScheduleVisitAsync(buyerId.Value, request, cancellationToken);
            return Ok(ApiResponse<VisitResponse>.Ok(result, "Visit scheduled successfully."));
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyVisits(CancellationToken cancellationToken)
    {
        var buyerId = GetUserId();
        if (buyerId is null)
            return Unauthorized();

        var result = await _visitService.GetMyVisitsAsync(buyerId.Value, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<VisitResponse>>.Ok(result));
    }

    [HttpGet("property/{propertyId:guid}")]
    public async Task<IActionResult> GetPropertyVisits(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _visitService.GetPropertyVisitsAsync(propertyId, userId.Value, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<VisitResponse>>.Ok(result));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{visitId:guid}/confirm")]
    [Authorize(Roles = "AGT,SEL,ADM,Admin")]
    public async Task<IActionResult> ConfirmVisit(
        Guid visitId,
        [FromBody] ConfirmVisitRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _visitService.ConfirmVisitAsync(visitId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<VisitResponse>.Ok(result, "Visit confirmed successfully."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{visitId:guid}/complete")]
    [Authorize(Roles = "AGT,ADM,Admin")]
    public async Task<IActionResult> CompleteVisit(
        Guid visitId,
        [FromBody] CompleteVisitRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _visitService.CompleteVisitAsync(visitId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<VisitResponse>.Ok(result, "Visit marked as completed."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{visitId:guid}/cancel")]
    public async Task<IActionResult> CancelVisit(
        Guid visitId,
        [FromBody] CancelVisitRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _visitService.CancelVisitAsync(visitId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<VisitResponse>.Ok(result, "Visit cancelled."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
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
