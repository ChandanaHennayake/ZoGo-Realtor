using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Buyer;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/buyer/properties")]
public sealed class BuyerPropertyController : ControllerBase
{
    private readonly IBuyerPropertyService _buyerPropertyService;

    public BuyerPropertyController(IBuyerPropertyService buyerPropertyService)
    {
        _buyerPropertyService = buyerPropertyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProperties(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] short? propertyTypeId = null,
        [FromQuery] short? listingTypeId = null,
        [FromQuery] short? districtId = null,
        [FromQuery] int? cityId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        CancellationToken cancellationToken = default)
    {
        Guid? userId = null;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (Guid.TryParse(userIdClaim, out var parsedId))
        {
            userId = parsedId;
        }

        var query = new BuyerPropertyListQuery
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber,
            PageSize = pageSize < 1 ? 20 : pageSize,
            Search = search,
            PropertyTypeId = propertyTypeId,
            ListingTypeId = listingTypeId,
            DistrictId = districtId,
            CityId = cityId,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };

        var items = await _buyerPropertyService.GetPropertiesAsync(
            query,
            userId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<BuyerPropertyResponse>>.Ok(items));
    }

    [HttpGet("{propertyId:guid}")]
    public async Task<IActionResult> GetById(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        Guid? userId = null;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (Guid.TryParse(userIdClaim, out var parsedId))
        {
            userId = parsedId;
        }

        var details = await _buyerPropertyService.GetPropertyDetailsAsync(
            propertyId,
            userId,
            cancellationToken);

        if (details is null)
        {
            return NotFound(ApiResponse.Fail("Property not found or is not available for viewing."));
        }

        return Ok(ApiResponse<BuyerPropertyDetailsResponse>.Ok(details));
    }
}
