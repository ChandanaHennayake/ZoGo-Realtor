using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Offers;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/admin/offers")]
[Authorize(Roles = "ADM,Admin")]
public sealed class AdminOfferController : ControllerBase
{
    private readonly IPropertyOfferService _offerService;

    public AdminOfferController(IPropertyOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOffers(
        [FromQuery] short? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _offerService.GetAllOffersAsync(status, pageNumber, pageSize, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<OfferResponse>>.Ok(result));
    }
}
