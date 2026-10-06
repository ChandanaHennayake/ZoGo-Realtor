using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Buyer;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/buyer/property-interactions")]
[Authorize]
public sealed class BuyerPropertyInteractionController : ControllerBase
{
    private readonly IBuyerPropertyInteractionService _interactionService;

    public BuyerPropertyInteractionController(IBuyerPropertyInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpPost("{propertyId:guid}")]
    public async Task<IActionResult> SetInteractionStatus(
        Guid propertyId,
        [FromBody] BuyerPropertyInteractionRequest request,
        CancellationToken cancellationToken)
    {
        var buyerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(buyerIdClaim, out var buyerId))
            return Unauthorized();

        try
        {
            var result = await _interactionService.SetInteractionStatusAsync(
                buyerId,
                propertyId,
                request.Status,
                cancellationToken);

            return Ok(ApiResponse<BuyerPropertyInteractionResponse>.Ok(result, "Interaction status updated successfully."));
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
    public async Task<IActionResult> UpdateInteractionStatus(
        Guid propertyId,
        [FromBody] BuyerPropertyInteractionRequest request,
        CancellationToken cancellationToken)
    {
        var buyerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(buyerIdClaim, out var buyerId))
            return Unauthorized();

        try
        {
            var result = await _interactionService.SetInteractionStatusAsync(
                buyerId,
                propertyId,
                request.Status,
                cancellationToken);

            return Ok(ApiResponse<BuyerPropertyInteractionResponse>.Ok(result, "Interaction status updated successfully."));
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
    public async Task<IActionResult> GetInteractionStatus(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var buyerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(buyerIdClaim, out var buyerId))
            return Unauthorized();

        var result = await _interactionService.GetInteractionAsync(
            buyerId,
            propertyId,
            cancellationToken);

        if (result is null)
            return NotFound(ApiResponse.Fail("No interaction recorded for this property."));

        return Ok(ApiResponse<BuyerPropertyInteractionResponse>.Ok(result));
    }
}
