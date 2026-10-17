using zogo.Application.DTOs.Transactions;
using zogo.Domain.Entities.Buyer;

namespace zogo.Application.Interfaces.Repositories;

public interface IPropertyTransactionRepository
{
    Task<PropertyTransaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<TransactionResponse?> GetTransactionDetailsAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<TransactionResponse?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransactionResponse>> GetAllTransactionsAsync(short? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<CommissionReportResponse> GetCommissionsReportAsync(CancellationToken cancellationToken = default);
    Task AddAsync(PropertyTransaction transaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(PropertyTransaction transaction, CancellationToken cancellationToken = default);
}
