using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Property.LegalDetails;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/properties/{propertyId:guid}/legal-details")]
[Authorize(Roles = "SEL")]
public class PropertyLegalDetailsController : ControllerBase
{
    private readonly IPropertyLegalDetailsService _legalDetailsService;

    public PropertyLegalDetailsController(
        IPropertyLegalDetailsService legalDetailsService)
    {
        _legalDetailsService = legalDetailsService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid propertyId,
        [FromBody] CreatePropertyLegalDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _legalDetailsService.CreateAsync(
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

        var result = await _legalDetailsService.GetByPropertyIdAsync(
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
        [FromBody] UpdatePropertyLegalDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _legalDetailsService.UpdateAsync(
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
            throw new UnauthorizedAccessException(
                "Invalid user identity.");

        return userId;
    }
}