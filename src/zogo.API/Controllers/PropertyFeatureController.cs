using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Property.PropertyFeature;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/properties/{propertyId:guid}/features")]
[Authorize(Roles = "SEL")]
public class PropertyFeatureController : ControllerBase
{
    private readonly IPropertyFeatureService _propertyFeatureService;

    public PropertyFeatureController(
        IPropertyFeatureService propertyFeatureService)
    {
        _propertyFeatureService = propertyFeatureService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        Guid propertyId,
        [FromBody] AddPropertyFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _propertyFeatureService.AddAsync(
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

        var result = await _propertyFeatureService.GetByPropertyIdAsync(
            userId,
            propertyId,
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{featureId:int}")]
    public async Task<IActionResult> Delete(
        Guid propertyId,
        int featureId,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _propertyFeatureService.DeleteAsync(
            userId,
            propertyId,
            featureId,
            cancellationToken);

        return NoContent();
    }
}