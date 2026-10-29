using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Common;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/common")]
[AllowAnonymous]
public class CommonController : ControllerBase
{
    private readonly ICommonService _commonService;

    public CommonController(ICommonService commonService)
    {
        _commonService = commonService;
    }

    [HttpGet("provinces")]
    public async Task<ActionResult<IReadOnlyList<ProvinceResponse>>> GetProvinces(
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetProvincesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("districts")]
    public async Task<ActionResult<IReadOnlyList<DistrictResponse>>> GetDistricts(
        [FromQuery] short? provinceId,
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetDistrictsAsync(provinceId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("districts/{districtId:int}/cities")]
    public async Task<ActionResult<IReadOnlyList<CityResponse>>> GetCities(
        short districtId,
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetCitiesByDistrictAsync(districtId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("districts/{districtId:int}/divisional-secretariats")]
    public async Task<ActionResult<IReadOnlyList<DivisionalSecretariatResponse>>> GetDivisionalSecretariats(
        short districtId,
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetDivisionalSecretariatsAsync(districtId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("divisional-secretariats/{dsId:int}/gn-divisions")]
    public async Task<ActionResult<IReadOnlyList<GramaNiladhariDivisionResponse>>> GetGramaNiladhariDivisions(
        int dsId,
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetGramaNiladhariDivisionsAsync(dsId, cancellationToken);
        return Ok(result);
    }
    [HttpGet("property-statuses")]
    public async Task<ActionResult<IReadOnlyList<PropertyStatusResponse>>> GetPropertyStatuses(
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetPropertyStatusesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("property-types")]
    public async Task<ActionResult<IReadOnlyList<PropertyTypeResponse>>> GetPropertyTypes(
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetPropertyTypesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("listing-types")]
    public async Task<ActionResult<IReadOnlyList<ListingTypeResponse>>> GetListingTypes(
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetListingTypesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("apartment-types")]
    public async Task<ActionResult<IReadOnlyList<ApartmentTypeResponse>>> GetApartmentTypes(
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetApartmentTypesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("furnishing-types")]
    public async Task<ActionResult<IReadOnlyList<FurnishingTypeResponse>>> GetFurnishingTypes(
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetFurnishingTypesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("view-types")]
    public async Task<ActionResult<IReadOnlyList<ViewTypeResponse>>> GetViewTypes(
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetViewTypesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("condominiums")]
    public async Task<ActionResult<IReadOnlyList<CondominiumResponse>>> GetCondominiums(
        [FromQuery] short? districtId,
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetCondominiumsAsync(districtId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("amenities")]
    public async Task<ActionResult<IReadOnlyList<AmenityMasterResponse>>> GetAmenities(
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetAmenitiesAsync(category, cancellationToken);
        return Ok(result);
    }

    [HttpGet("features")]
    public async Task<ActionResult<IReadOnlyList<FeatureMasterResponse>>> GetFeatures(
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetFeaturesAsync(category, cancellationToken);
        return Ok(result);
    }

    [HttpGet("document-types")]
    public async Task<ActionResult<IReadOnlyList<DocumentTypeResponse>>> GetDocumentTypes(
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetDocumentTypesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("ownership-types")]
    public async Task<ActionResult<IReadOnlyList<OwnershipTypeResponse>>> GetOwnershipTypes(
        CancellationToken cancellationToken)
    {
        var result = await _commonService.GetOwnershipTypesAsync(cancellationToken);
        return Ok(result);
    }
}

