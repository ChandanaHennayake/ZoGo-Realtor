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
        var buyerId = GetUserId();
        if (buyerId is null)
            return Unauthorized();

        try
        {
            var result = await _interactionService.SetInteractionStatusAsync(
                buyerId.Value,
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
        var buyerId = GetUserId();
        if (buyerId is null)
            return Unauthorized();

        try
        {
            var result = await _interactionService.SetInteractionStatusAsync(
                buyerId.Value,
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

    [HttpPut("{propertyId:guid}/documents-request")]
    public async Task<IActionResult> RequestDocuments(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var buyerId = GetUserId();
        if (buyerId is null)
            return Unauthorized();

        try
        {
            var result = await _interactionService.RequestDocumentsAsync(
                buyerId.Value,
                propertyId,
                cancellationToken);

            return Ok(ApiResponse<BuyerPropertyInteractionResponse>.Ok(result, "Documents requested successfully."));
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
        var buyerId = GetUserId();
        if (buyerId is null)
            return Unauthorized();

        var result = await _interactionService.GetInteractionAsync(
            buyerId.Value,
            propertyId,
            cancellationToken);

        if (result is null)
            return NotFound(ApiResponse.Fail("No interaction recorded for this property."));

        return Ok(ApiResponse<BuyerPropertyInteractionResponse>.Ok(result));
    }

    [HttpPut("{propertyId:guid}/sold")]
    [Authorize(Roles = "AGT,ADM,Admin")]
    public async Task<IActionResult> MarkSold(
        Guid propertyId,
        [FromBody] MarkSoldRequest? request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _interactionService.MarkSoldAsync(
                userId.Value,
                propertyId,
                request?.BuyerId,
                cancellationToken);

            return Ok(ApiResponse<BuyerPropertyInteractionResponse>.Ok(result, "Property and inquiry marked as sold successfully."));
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

    [HttpPut("{propertyId:guid}/reject")]
    [Authorize(Roles = "AGT,ADM,Admin")]
    public async Task<IActionResult> RejectInquiry(
        Guid propertyId,
        [FromBody] RejectInquiryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(request?.Reason))
            return BadRequest(ApiResponse.Fail("A rejection reason is required."));

        try
        {
            var result = await _interactionService.RejectInquiryAsync(
                userId.Value,
                propertyId,
                request.BuyerId,
                request.Reason,
                cancellationToken);

            return Ok(ApiResponse<BuyerPropertyInteractionResponse>.Ok(result, "Inquiry rejected successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{propertyId:guid}/cancel")]
    public async Task<IActionResult> CancelInquiry(
        Guid propertyId,
        [FromBody] CancelInquiryRequest? request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _interactionService.CancelInquiryAsync(
                userId.Value,
                propertyId,
                request?.BuyerId,
                request?.Reason,
                cancellationToken);

            return Ok(ApiResponse<BuyerPropertyInteractionResponse>.Ok(result, "Inquiry cancelled successfully."));
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
