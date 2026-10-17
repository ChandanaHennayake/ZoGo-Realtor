using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Admin;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/admin/properties")]
[Authorize(Roles = "ADM,Admin")]
public sealed class AdminPropertyController : ControllerBase
{
    private readonly IAdminPropertyService _adminPropertyService;

    public AdminPropertyController(IAdminPropertyService adminPropertyService)
    {
        _adminPropertyService = adminPropertyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProperties(
        [FromQuery] short? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminPropertyService.GetPropertiesByStatusAsync(
            status,
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<AdminPropertySummaryResponse>>.Ok(result));
    }

    [HttpGet("{propertyId:guid}")]
    public async Task<IActionResult> GetPropertyReviewDetails(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminPropertyService.GetPropertyReviewDetailsAsync(
            propertyId,
            cancellationToken);

        if (result == null)
            return NotFound(ApiResponse.Fail("Property not found."));

        return Ok(ApiResponse<AdminPropertyReviewDetailsResponse>.Ok(result));
    }

    [HttpPut("{propertyId:guid}/start-review")]
    public async Task<IActionResult> StartReview(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var adminUserId = GetUserId();
        if (adminUserId == null)
            return Unauthorized();

        try
        {
            await _adminPropertyService.StartReviewAsync(propertyId, adminUserId.Value, cancellationToken);
            return Ok(ApiResponse<string>.Ok("Property is now under review."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{propertyId:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var adminUserId = GetUserId();
        if (adminUserId == null)
            return Unauthorized();

        try
        {
            await _adminPropertyService.ApprovePropertyAsync(propertyId, adminUserId.Value, cancellationToken);
            return Ok(ApiResponse<string>.Ok("Property has been approved and published."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{propertyId:guid}/request-changes")]
    public async Task<IActionResult> RequestChanges(
        Guid propertyId,
        [FromBody] RequestPropertyChangesRequest request,
        CancellationToken cancellationToken = default)
    {
        var adminUserId = GetUserId();
        if (adminUserId == null)
            return Unauthorized();

        try
        {
            await _adminPropertyService.RequestChangesAsync(propertyId, request, adminUserId.Value, cancellationToken);
            return Ok(ApiResponse<string>.Ok("Changes requested from the seller successfully."));
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

    [HttpPut("{propertyId:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid propertyId,
        [FromBody] RejectPropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        var adminUserId = GetUserId();
        if (adminUserId == null)
            return Unauthorized();

        try
        {
            await _adminPropertyService.RejectPropertyAsync(propertyId, request, adminUserId.Value, cancellationToken);
            return Ok(ApiResponse<string>.Ok("Property listing rejected."));
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
