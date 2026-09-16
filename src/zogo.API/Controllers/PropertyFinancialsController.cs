using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Property.Financials;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/properties/{propertyId:guid}/financials")]
[Authorize(Roles = "SEL")]
public class PropertyFinancialsController : ControllerBase
{
    private readonly IPropertyFinancialsService _propertyFinancialsService;

    public PropertyFinancialsController(
        IPropertyFinancialsService propertyFinancialsService)
    {
        _propertyFinancialsService = propertyFinancialsService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid propertyId,
        [FromBody] CreatePropertyFinancialsRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _propertyFinancialsService.CreateAsync(
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
        var userId = GetUserId();

        var result = await _propertyFinancialsService.GetByPropertyIdAsync(
            userId,
            propertyId,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        Guid propertyId,
        [FromBody] UpdatePropertyFinancialsRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _propertyFinancialsService.UpdateAsync(
            userId,
            propertyId,
            request,
            cancellationToken);

        return Ok(result);
    }

    private Guid GetUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return userId;
    }
}