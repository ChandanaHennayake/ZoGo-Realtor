using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Transactions;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/transactions")]
[Authorize]
public sealed class PropertyTransactionController : ControllerBase
{
    private readonly IPropertyTransactionService _transactionService;

    public PropertyTransactionController(IPropertyTransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    [Authorize(Roles = "SEL,AGT,ADM,Admin")]
    public async Task<IActionResult> InitiateTransaction(
        [FromBody] InitiateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _transactionService.InitiateTransactionAsync(request, userId.Value, cancellationToken);
            return Ok(ApiResponse<TransactionResponse>.Ok(result, "Transaction initiated for legal closing."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPut("{transactionId:guid}/verify-title")]
    [Authorize(Roles = "AGT,ADM,Admin")]
    public async Task<IActionResult> VerifyTitle(
        Guid transactionId,
        [FromBody] VerifyTitleRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _transactionService.VerifyTitleAsync(transactionId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<TransactionResponse>.Ok(result, "Title deed verified by legal counsel."));
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

    [HttpPut("{transactionId:guid}/sign-deed")]
    [Authorize(Roles = "SEL,AGT,ADM,Admin")]
    public async Task<IActionResult> SignDeed(
        Guid transactionId,
        [FromBody] SignDeedRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _transactionService.SignDeedAsync(transactionId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<TransactionResponse>.Ok(result, "Deed of Transfer signed before notary public."));
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

    [HttpPut("{transactionId:guid}/settle")]
    [Authorize(Roles = "ADM,Admin")]
    public async Task<IActionResult> SettleTransaction(
        Guid transactionId,
        [FromBody] SettleTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _transactionService.SettleTransactionAsync(transactionId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<TransactionResponse>.Ok(result, "Transaction fully settled. Property officially marked as SOLD."));
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

    [HttpPut("{transactionId:guid}/cancel")]
    [Authorize(Roles = "SEL,ADM,Admin")]
    public async Task<IActionResult> CancelTransaction(
        Guid transactionId,
        [FromBody] CancelTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _transactionService.CancelTransactionAsync(transactionId, request, userId.Value, cancellationToken);
            return Ok(ApiResponse<TransactionResponse>.Ok(result, "Transaction cancelled."));
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

    [HttpGet("{transactionId:guid}")]
    public async Task<IActionResult> GetTransactionDetails(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        var result = await _transactionService.GetTransactionDetailsAsync(transactionId, cancellationToken);
        if (result == null)
            return NotFound(ApiResponse.Fail("Transaction not found."));

        return Ok(ApiResponse<TransactionResponse>.Ok(result));
    }

    [HttpGet("property/{propertyId:guid}")]
    public async Task<IActionResult> GetPropertyTransaction(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var result = await _transactionService.GetPropertyTransactionAsync(propertyId, cancellationToken);
        if (result == null)
            return NotFound(ApiResponse.Fail("No active transaction for this property."));

        return Ok(ApiResponse<TransactionResponse>.Ok(result));
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
