using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using zogo.Application.DTOs.Properties.PropertyAmenity;
using zogo.Application.DTOs.Property.PropertyAmenity;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/properties/{propertyId:guid}/amenities")]
[Authorize(Roles = "SEL")]
public class PropertyAmenityController : ControllerBase
{
    private readonly IPropertyAmenityService _propertyAmenityService;

    public PropertyAmenityController(
        IPropertyAmenityService propertyAmenityService)
    {
        _propertyAmenityService = propertyAmenityService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        Guid propertyId,
        [FromBody] AddPropertyAmenityRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _propertyAmenityService.AddAsync(
            userId,
            propertyId,
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _propertyAmenityService.GetByPropertyIdAsync(
            userId,
            propertyId,
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{amenityId:int}")]
    public async Task<IActionResult> Delete(
        Guid propertyId,
        int amenityId,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _propertyAmenityService.DeleteAsync(
            userId,
            propertyId,
            amenityId,
            cancellationToken);

        return NoContent();
    }
}