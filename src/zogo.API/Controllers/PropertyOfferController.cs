using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Offers;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/offers")]
[Authorize]
public sealed class PropertyOfferController : ControllerBase
{
    private readonly IPropertyOfferService _offerService;

    public PropertyOfferController(IPropertyOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOffer(
        [FromBody] CreateOfferRequest request,
        CancellationToken cancellationToken)
    {
        var buyerId = GetUserId();
        if (buyerId is null)
            return Unauthorized();

        try
        {
            var result = await _offerService.SubmitOfferAsync(buyerId.Value, request, cancellationToken);
            return Ok(ApiResponse<OfferResponse>.Ok(result, "Offer submitted successfully."));
        }
        catch (NotFoundException ex)
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

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOffers(CancellationToken cancellationToken)
    {
        var buyerId = GetUserId();
        if (buyerId is null)
            return Unauthorized();

        var result = await _offerService.GetMyOffersAsync(buyerId.Value, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<OfferResponse>>.Ok(result));
    }

    [HttpGet("property/{propertyId:guid}")]
    public async Task<IActionResult> GetPropertyOffers(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _offerService.GetPropertyOffersAsync(propertyId, userId.Value, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<OfferResponse>>.Ok(result));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{offerId:guid}/counter")]
    [Authorize(Roles = "SEL,AGT,ADM,Admin")]
    public async Task<IActionResult> CounterOffer(
        Guid offerId,
        [FromBody] CounterOfferRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _offerService.CounterOfferAsync(offerId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<OfferResponse>.Ok(result, "Counter offer submitted."));
        }
        catch (NotFoundException ex)
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

    [HttpPut("{offerId:guid}/accept")]
    [Authorize(Roles = "SEL,ADM,Admin")]
    public async Task<IActionResult> AcceptOffer(
        Guid offerId,
        [FromBody] AcceptOfferRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _offerService.AcceptOfferAsync(offerId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<OfferResponse>.Ok(result, "Offer accepted successfully. Deal advanced to transaction stage."));
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

    [HttpPut("{offerId:guid}/reject")]
    [Authorize(Roles = "SEL,AGT,ADM,Admin")]
    public async Task<IActionResult> RejectOffer(
        Guid offerId,
        [FromBody] RejectOfferRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _offerService.RejectOfferAsync(offerId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<OfferResponse>.Ok(result, "Offer rejected."));
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

    [HttpPut("{offerId:guid}/withdraw")]
    public async Task<IActionResult> WithdrawOffer(
        Guid offerId,
        [FromBody] WithdrawOfferRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _offerService.WithdrawOfferAsync(offerId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<OfferResponse>.Ok(result, "Offer withdrawn."));
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
