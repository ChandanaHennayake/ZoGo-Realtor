using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Properties;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/properties/{propertyId:guid}/apartment-details")]
[Authorize(Roles = "SEL")]

public sealed class ApartmentDetailsController : ControllerBase
{
    private readonly IApartmentDetailsService _apartmentDetailsService;

    public ApartmentDetailsController(
        IApartmentDetailsService apartmentDetailsService)
    {
        _apartmentDetailsService = apartmentDetailsService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid propertyId,
        [FromBody] CreateApartmentDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _apartmentDetailsService.CreateAsync(
                userId.Value,
                propertyId,
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetByPropertyId),
                new { propertyId },
                result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByPropertyId(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var result =
            await _apartmentDetailsService.GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        Guid propertyId,
        [FromBody] UpdateApartmentDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        try
        {
            var updated =
                await _apartmentDetailsService.UpdateAsync(
                    userId.Value,
                    propertyId,
                    request,
                    cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
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