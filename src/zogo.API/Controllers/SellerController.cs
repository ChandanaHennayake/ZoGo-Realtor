using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/seller")]
[Authorize]
public class SellerController : ControllerBase
{
    private readonly ISellerService _sellerService;

    public SellerController(
        ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    [HttpPost("activate")]
    public async Task<IActionResult> ActivateSeller(
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
            await _sellerService.ActivateSellerAsync(
                userId,
                cancellationToken);

        return Ok(result);
    }
}