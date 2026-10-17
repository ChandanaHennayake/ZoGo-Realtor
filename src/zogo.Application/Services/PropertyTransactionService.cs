using zogo.Application.DTOs.Transactions;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Entities.Notification;

namespace zogo.Application.Services;

public sealed class PropertyTransactionService : IPropertyTransactionService
{
    private readonly IPropertyTransactionRepository _transactionRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserNotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PropertyTransactionService(
        IPropertyTransactionRepository transactionRepository,
        IPropertyRepository propertyRepository,
        IUserNotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _propertyRepository = propertyRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TransactionResponse> InitiateTransactionAsync(
        InitiateTransactionRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property == null || property.DeletedAt.HasValue)
            throw new NotFoundException("Property", request.PropertyId);

        var tx = PropertyTransaction.Initiate(
            request.PropertyId,
            request.OfferId,
            request.BuyerId,
            property.OwnerUserId,
            request.AgentId,
            request.AgreedPrice,
            request.CommissionRate,
            request.Notes);

        await _transactionRepository.AddAsync(tx, cancellationToken);

        // Notify Buyer & Seller
        var buyerNotif = UserNotification.Create(
            request.BuyerId,
            "TRANSACTION_INITIATED",
            "Closing Pipeline Initiated",
            $"Closing process initiated for '{property.Title}'. Title deed verification is underway.",
            tx.Id.ToString(),
            "Transaction");
        var sellerNotif = UserNotification.Create(
            property.OwnerUserId,
            "TRANSACTION_INITIATED",
            "Closing Pipeline Initiated",
            $"Closing process initiated for '{property.Title}' at agreed price LKR {request.AgreedPrice:N0}.",
            tx.Id.ToString(),
            "Transaction");

        await _notificationRepository.AddAsync(buyerNotif, cancellationToken);
        await _notificationRepository.AddAsync(sellerNotif, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var res = await _transactionRepository.GetTransactionDetailsAsync(tx.Id, cancellationToken);
        return res!;
    }

    public async Task<TransactionResponse> VerifyTitleAsync(
        Guid transactionId,
        VerifyTitleRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tx = await _transactionRepository.GetByIdAsync(transactionId, cancellationToken);
        if (tx == null || tx.DeletedAt.HasValue)
            throw new NotFoundException("PropertyTransaction", transactionId);

        tx.VerifyTitle(request.TitleDeedReference, request.NotaryName, request.Notes);
        await _transactionRepository.UpdateAsync(tx, cancellationToken);

        var buyerNotif = UserNotification.Create(
            tx.BuyerId,
            "TRANSACTION_TITLE_VERIFIED",
            "Title Deed Verified",
            $"Legal title deed has been verified by notary {request.NotaryName}. Ready for deed signing.",
            tx.Id.ToString(),
            "Transaction");
        var sellerNotif = UserNotification.Create(
            tx.SellerId,
            "TRANSACTION_TITLE_VERIFIED",
            "Title Deed Verified",
            $"Title deed verified by notary {request.NotaryName}. Ready for deed signing.",
            tx.Id.ToString(),
            "Transaction");

        await _notificationRepository.AddAsync(buyerNotif, cancellationToken);
        await _notificationRepository.AddAsync(sellerNotif, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var res = await _transactionRepository.GetTransactionDetailsAsync(tx.Id, cancellationToken);
        return res!;
    }

    public async Task<TransactionResponse> SignDeedAsync(
        Guid transactionId,
        SignDeedRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var tx = await _transactionRepository.GetByIdAsync(transactionId, cancellationToken);
        if (tx == null || tx.DeletedAt.HasValue)
            throw new NotFoundException("PropertyTransaction", transactionId);

        tx.SignDeed(request?.Notes);
        await _transactionRepository.UpdateAsync(tx, cancellationToken);

        var buyerNotif = UserNotification.Create(
            tx.BuyerId,
            "TRANSACTION_DEED_SIGNED",
            "Deed Signed! 📜",
            "Legal deeds have been signed. Final settlement is now pending.",
            tx.Id.ToString(),
            "Transaction");
        var sellerNotif = UserNotification.Create(
            tx.SellerId,
            "TRANSACTION_DEED_SIGNED",
            "Deed Signed! 📜",
            "Legal deeds have been signed. Final settlement is now pending.",
            tx.Id.ToString(),
            "Transaction");

        await _notificationRepository.AddAsync(buyerNotif, cancellationToken);
        await _notificationRepository.AddAsync(sellerNotif, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var res = await _transactionRepository.GetTransactionDetailsAsync(tx.Id, cancellationToken);
        return res!;
    }

    public async Task<TransactionResponse> SettleTransactionAsync(
        Guid transactionId,
        SettleTransactionRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var tx = await _transactionRepository.GetByIdAsync(transactionId, cancellationToken);
        if (tx == null || tx.DeletedAt.HasValue)
            throw new NotFoundException("PropertyTransaction", transactionId);

        tx.Settle(request?.ClosingNotes);
        await _transactionRepository.UpdateAsync(tx, cancellationToken);

        // Transition property to SOLD (5)
        var property = await _propertyRepository.GetByIdAsync(tx.PropertyId, cancellationToken);
        if (property != null)
        {
            property.MarkSold(userId);
        }

        var buyerNotif = UserNotification.Create(
            tx.BuyerId,
            "TRANSACTION_SETTLED",
            "Deal Settled & Completed! 🏆",
            $"Congratulations! Deal on '{property?.Title ?? "Property"}' is settled. Property officially transferred.",
            tx.Id.ToString(),
            "Transaction");
        var sellerNotif = UserNotification.Create(
            tx.SellerId,
            "TRANSACTION_SETTLED",
            "Deal Settled & Funds Distributed! 💰",
            $"Congratulations! Settlement completed for '{property?.Title ?? "Property"}' at LKR {tx.AgreedPrice:N0}.",
            tx.Id.ToString(),
            "Transaction");

        await _notificationRepository.AddAsync(buyerNotif, cancellationToken);
        await _notificationRepository.AddAsync(sellerNotif, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var res = await _transactionRepository.GetTransactionDetailsAsync(tx.Id, cancellationToken);
        return res!;
    }

    public async Task<TransactionResponse> CancelTransactionAsync(
        Guid transactionId,
        CancelTransactionRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tx = await _transactionRepository.GetByIdAsync(transactionId, cancellationToken);
        if (tx == null || tx.DeletedAt.HasValue)
            throw new NotFoundException("PropertyTransaction", transactionId);

        tx.Cancel(request.Reason);
        await _transactionRepository.UpdateAsync(tx, cancellationToken);

        var buyerNotif = UserNotification.Create(
            tx.BuyerId,
            "TRANSACTION_CANCELLED",
            "Transaction Cancelled",
            $"Closing transaction was cancelled: {request.Reason}",
            tx.Id.ToString(),
            "Transaction");
        var sellerNotif = UserNotification.Create(
            tx.SellerId,
            "TRANSACTION_CANCELLED",
            "Transaction Cancelled",
            $"Closing transaction was cancelled: {request.Reason}",
            tx.Id.ToString(),
            "Transaction");

        await _notificationRepository.AddAsync(buyerNotif, cancellationToken);
        await _notificationRepository.AddAsync(sellerNotif, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var res = await _transactionRepository.GetTransactionDetailsAsync(tx.Id, cancellationToken);
        return res!;
    }

    public async Task<TransactionResponse?> GetTransactionDetailsAsync(
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        return await _transactionRepository.GetTransactionDetailsAsync(transactionId, cancellationToken);
    }

    public async Task<TransactionResponse?> GetPropertyTransactionAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await _transactionRepository.GetByPropertyIdAsync(propertyId, cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionResponse>> GetAllTransactionsAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 50 : pageSize;
        return await _transactionRepository.GetAllTransactionsAsync(status, page, size, cancellationToken);
    }

    public async Task<CommissionReportResponse> GetCommissionsReportAsync(CancellationToken cancellationToken = default)
    {
        return await _transactionRepository.GetCommissionsReportAsync(cancellationToken);
    }
}
