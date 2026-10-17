using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Admin;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/admin/dashboard")]
[Authorize(Roles = "ADM,Admin")]
public sealed class AdminDashboardController : ControllerBase
{
    private readonly IAdminPropertyService _adminPropertyService;

    public AdminDashboardController(IAdminPropertyService adminPropertyService)
    {
        _adminPropertyService = adminPropertyService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken = default)
    {
        var stats = await _adminPropertyService.GetDashboardStatsAsync(cancellationToken);
        return Ok(ApiResponse<AdminDashboardStatsResponse>.Ok(stats));
    }
}
