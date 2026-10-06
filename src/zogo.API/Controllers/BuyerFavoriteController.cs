using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Properties;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/buyer/favorites")]
[Authorize]
public sealed class BuyerFavoriteController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public BuyerFavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpPost("{propertyId:guid}")]
    public async Task<IActionResult> AddFavorite(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            var added = await _favoriteService.AddFavoriteAsync(
                userId,
                propertyId,
                cancellationToken);

            var message = added
                ? "Property added to favorites."
                : "Property already in favorites.";

            return Ok(ApiResponse.Ok(message));
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

    [HttpDelete("{propertyId:guid}")]
    public async Task<IActionResult> RemoveFavorite(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            var removed = await _favoriteService.RemoveFavoriteAsync(
                userId,
                propertyId,
                cancellationToken);

            if (!removed)
                return NotFound(ApiResponse.Fail("Property is not in your favorites."));

            return Ok(ApiResponse.Ok("Property removed from favorites."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyFavorites(
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _favoriteService.GetFavoritesAsync(
            userId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetPropertyResponse>>.Ok(result));
    }
}
