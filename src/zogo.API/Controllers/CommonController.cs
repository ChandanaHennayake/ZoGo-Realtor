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
} 
