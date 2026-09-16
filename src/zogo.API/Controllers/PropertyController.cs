using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Properties;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/properties")]
[Authorize(Roles = "SEL")]
public sealed class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(
        IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDraft(
        [FromBody] CreatePropertyRequest request,
        CancellationToken cancellationToken)
    {
      
      

        var userIdClaim =
          User.FindFirstValue(ClaimTypes.NameIdentifier)
          ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }


        var result =
            await _propertyService.CreateDraftAsync(
                userId,
                request,
                cancellationToken);

        return Created(
            $"/api/v1/properties/{result.PropertyId}",
            result);
    }



    [HttpGet("{propertyId:guid}")]
    public async Task<IActionResult> GetById(
    Guid propertyId,
    CancellationToken cancellationToken)
    {
        var result = await _propertyService.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyProperties(
    CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _propertyService.GetMyPropertiesAsync(
            userId,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{propertyId:guid}")]
    public async Task<IActionResult> Update(
    Guid propertyId,
    [FromBody] UpdatePropertyRequest request,
    CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var updated = await _propertyService.UpdateAsync(
                userId,
                propertyId,
                request,
                cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

}