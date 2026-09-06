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
}