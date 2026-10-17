using zogo.Application.DTOs.Transactions;

namespace zogo.Application.Interfaces.Services;

public interface IPropertyTransactionService
{
    Task<TransactionResponse> InitiateTransactionAsync(InitiateTransactionRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<TransactionResponse> VerifyTitleAsync(Guid transactionId, VerifyTitleRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<TransactionResponse> SignDeedAsync(Guid transactionId, SignDeedRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<TransactionResponse> SettleTransactionAsync(Guid transactionId, SettleTransactionRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<TransactionResponse> CancelTransactionAsync(Guid transactionId, CancelTransactionRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<TransactionResponse?> GetTransactionDetailsAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<TransactionResponse?> GetPropertyTransactionAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransactionResponse>> GetAllTransactionsAsync(short? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<CommissionReportResponse> GetCommissionsReportAsync(CancellationToken cancellationToken = default);
}
