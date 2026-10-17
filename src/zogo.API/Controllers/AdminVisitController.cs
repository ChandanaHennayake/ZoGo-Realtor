using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Visits;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/admin/visits")]
[Authorize(Roles = "ADM,Admin")]
public sealed class AdminVisitController : ControllerBase
{
    private readonly IPropertyVisitService _visitService;

    public AdminVisitController(IPropertyVisitService visitService)
    {
        _visitService = visitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVisits(
        [FromQuery] short? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _visitService.GetAllVisitsAsync(status, pageNumber, pageSize, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<VisitResponse>>.Ok(result));
    }
}
