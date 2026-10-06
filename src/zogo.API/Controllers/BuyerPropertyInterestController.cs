using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Buyer;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/buyer/property-interests")]
[Authorize]
public sealed class BuyerPropertyInterestController : ControllerBase
{
    private readonly IPropertyInterestService _propertyInterestService;

    public BuyerPropertyInterestController(IPropertyInterestService propertyInterestService)
    {
        _propertyInterestService = propertyInterestService;
    }

    [HttpPost]
    public async Task<IActionResult> SetInterest(
        [FromBody] SetPropertyInterestRequest request,
        CancellationToken cancellationToken)
    {
        var buyerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(buyerIdClaim, out var buyerId))
            return Unauthorized();

        if (request.PropertyId is null || request.PropertyId == Guid.Empty)
            return BadRequest(ApiResponse.Fail("PropertyId is required."));

        try
        {
            var result = await _propertyInterestService.SetInterestAsync(
                buyerId,
                request.PropertyId.Value,
                request.Status,
                cancellationToken);

            return Ok(ApiResponse<PropertyInterestResponse>.Ok(result, "Property interest updated successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
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

    [HttpPost("{propertyId:guid}")]
    public async Task<IActionResult> SetInterestWithRoute(
        Guid propertyId,
        [FromBody] UpdatePropertyInterestRequest request,
        CancellationToken cancellationToken)
    {
        var buyerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(buyerIdClaim, out var buyerId))
            return Unauthorized();

        try
        {
            var result = await _propertyInterestService.SetInterestAsync(
                buyerId,
                propertyId,
                request.Status,
                cancellationToken);

            return Ok(ApiResponse<PropertyInterestResponse>.Ok(result, "Property interest updated successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
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

    [HttpPut("{propertyId:guid}")]
    public async Task<IActionResult> UpdateInterest(
        Guid propertyId,
        [FromBody] UpdatePropertyInterestRequest request,
        CancellationToken cancellationToken)
    {
        var buyerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(buyerIdClaim, out var buyerId))
            return Unauthorized();

        try
        {
            var result = await _propertyInterestService.SetInterestAsync(
                buyerId,
                propertyId,
                request.Status,
                cancellationToken);

            return Ok(ApiResponse<PropertyInterestResponse>.Ok(result, "Property interest updated successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
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

    [HttpGet("{propertyId:guid}")]
    public async Task<IActionResult> GetMyInterest(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var buyerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(buyerIdClaim, out var buyerId))
            return Unauthorized();

        var result = await _propertyInterestService.GetInterestAsync(
            buyerId,
            propertyId,
            cancellationToken);

        if (result is null)
            return NotFound(ApiResponse.Fail("No interest recorded for this property."));

        return Ok(ApiResponse<PropertyInterestResponse>.Ok(result));
    }
}
