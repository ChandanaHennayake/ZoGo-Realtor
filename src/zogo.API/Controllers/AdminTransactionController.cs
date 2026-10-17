using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Transactions;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/admin/transactions")]
[Authorize(Roles = "ADM,Admin")]
public sealed class AdminTransactionController : ControllerBase
{
    private readonly IPropertyTransactionService _transactionService;

    public AdminTransactionController(IPropertyTransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions(
        [FromQuery] short? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _transactionService.GetAllTransactionsAsync(status, pageNumber, pageSize, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TransactionResponse>>.Ok(result));
    }

    [HttpGet("commissions")]
    public async Task<IActionResult> GetCommissionsReport(CancellationToken cancellationToken = default)
    {
        var report = await _transactionService.GetCommissionsReportAsync(cancellationToken);
        return Ok(ApiResponse<CommissionReportResponse>.Ok(report));
    }
}
