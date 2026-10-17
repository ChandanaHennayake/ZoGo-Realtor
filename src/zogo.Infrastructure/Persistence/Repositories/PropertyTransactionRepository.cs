using Microsoft.EntityFrameworkCore;
using zogo.Application.DTOs.Transactions;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Buyer;

namespace zogo.Infrastructure.Persistence.Repositories;

public sealed class PropertyTransactionRepository : IPropertyTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PropertyTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        await _context.PropertyTransactions.AddAsync(transaction, cancellationToken);
    }

    public async Task UpdateAsync(PropertyTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        _context.PropertyTransactions.Update(transaction);
        await Task.CompletedTask;
    }

    public async Task<PropertyTransaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyTransactions
            .Include(t => t.Property)
            .Include(t => t.Buyer)
            .Include(t => t.Seller)
            .Include(t => t.Agent)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.DeletedAt == null, cancellationToken);
    }

    public async Task<TransactionResponse?> GetTransactionDetailsAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var item = await _context.PropertyTransactions
            .AsNoTracking()
            .Include(t => t.Property)
            .Include(t => t.Buyer)
            .Include(t => t.Seller)
            .Include(t => t.Agent)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.DeletedAt == null, cancellationToken);

        if (item == null) return null;

        var mapped = await MapTransactionsToResponse(new List<PropertyTransaction> { item }, cancellationToken);
        return mapped.FirstOrDefault();
    }

    public async Task<TransactionResponse?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        var item = await _context.PropertyTransactions
            .AsNoTracking()
            .Include(t => t.Property)
            .Include(t => t.Buyer)
            .Include(t => t.Seller)
            .Include(t => t.Agent)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(t => t.PropertyId == propertyId && t.DeletedAt == null, cancellationToken);

        if (item == null) return null;

        var mapped = await MapTransactionsToResponse(new List<PropertyTransaction> { item }, cancellationToken);
        return mapped.FirstOrDefault();
    }

    public async Task<IReadOnlyList<TransactionResponse>> GetAllTransactionsAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var queryable = _context.PropertyTransactions
            .AsNoTracking()
            .Include(t => t.Property)
            .Include(t => t.Buyer)
            .Include(t => t.Seller)
            .Include(t => t.Agent)
            .Where(t => t.DeletedAt == null);

        if (status.HasValue && status.Value > 0)
        {
            queryable = queryable.Where(t => t.Status == status.Value);
        }

        var list = await queryable
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return await MapTransactionsToResponse(list, cancellationToken);
    }

    public async Task<CommissionReportResponse> GetCommissionsReportAsync(CancellationToken cancellationToken = default)
    {
        var allTx = await _context.PropertyTransactions
            .AsNoTracking()
            .Include(t => t.Agent)
            .Where(t => t.DeletedAt == null)
            .ToListAsync(cancellationToken);

        var settled = allTx.Where(t => t.Status == 4).ToList();
        var active = allTx.Where(t => t.Status >= 1 && t.Status <= 3).ToList();

        var totalVolume = settled.Sum(t => t.AgreedPrice);
        var totalCommission = settled.Sum(t => t.CommissionAmount);

        // Group by agent
        var agentGroups = settled
            .Where(t => t.AgentId.HasValue && t.Agent != null)
            .GroupBy(t => t.AgentId!.Value)
            .Select(g =>
            {
                var first = g.First();
                return new AgentCommissionItem
                {
                    AgentId = g.Key,
                    AgentName = $"{first.Agent!.FirstName} {first.Agent.LastName}".Trim(),
                    Email = first.Agent.Email ?? string.Empty,
                    DealsClosed = g.Count(),
                    TotalSalesVolume = g.Sum(x => x.AgreedPrice),
                    CommissionEarned = g.Sum(x => x.CommissionAmount)
                };
            })
            .OrderByDescending(a => a.CommissionEarned)
            .ToList();

        return new CommissionReportResponse
        {
            TotalSettledVolume = totalVolume,
            TotalCommissionEarned = totalCommission,
            CompletedDealsCount = settled.Count,
            ActiveTransactionsCount = active.Count,
            AgentBreakdown = agentGroups
        };
    }

    private async Task<IReadOnlyList<TransactionResponse>> MapTransactionsToResponse(
        List<PropertyTransaction> transactions,
        CancellationToken cancellationToken)
    {
        if (transactions.Count == 0) return Array.Empty<TransactionResponse>();

        var districtIds = transactions.Where(t => t.Property != null).Select(t => t.Property!.DistrictId).Distinct().ToList();
        var cityIds = transactions.Where(t => t.Property != null).Select(t => t.Property!.CityId).Distinct().ToList();

        var districts = await _context.Districts
            .AsNoTracking()
            .Where(d => districtIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var cities = await _context.Cities
            .AsNoTracking()
            .Where(c => cityIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        return transactions.Select(t =>
        {
            districts.TryGetValue(t.Property?.DistrictId ?? 0, out var districtName);
            cities.TryGetValue(t.Property?.CityId ?? 0, out var cityName);

            return new TransactionResponse
            {
                TransactionId = t.Id,
                PropertyId = t.PropertyId,
                PropertyTitle = t.Property?.Title ?? "Property Listing",
                PropertyReferenceNo = t.Property?.ReferenceNo ?? string.Empty,
                DistrictName = districtName ?? string.Empty,
                CityName = cityName ?? string.Empty,
                OfferId = t.OfferId,
                BuyerId = t.BuyerId,
                BuyerName = t.Buyer != null ? $"{t.Buyer.FirstName} {t.Buyer.LastName}".Trim() : "Buyer",
                BuyerEmail = t.Buyer?.Email ?? string.Empty,
                BuyerPhone = t.Buyer?.PhoneNumber ?? string.Empty,
                SellerId = t.SellerId,
                SellerName = t.Seller != null ? $"{t.Seller.FirstName} {t.Seller.LastName}".Trim() : "Seller",
                SellerEmail = t.Seller?.Email ?? string.Empty,
                SellerPhone = t.Seller?.PhoneNumber ?? string.Empty,
                AgentId = t.AgentId,
                AgentName = t.Agent != null ? $"{t.Agent.FirstName} {t.Agent.LastName}".Trim() : null,
                AgreedPrice = t.AgreedPrice,
                CommissionRate = t.CommissionRate,
                CommissionAmount = t.CommissionAmount,
                Status = t.Status,
                StatusName = ResolveTransactionStatusName(t.Status),
                TitleDeedReference = t.TitleDeedReference,
                NotaryName = t.NotaryName,
                ClosingNotes = t.ClosingNotes,
                ClosingDate = t.ClosingDate,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            };
        }).ToList();
    }

    private static string ResolveTransactionStatusName(short status) => status switch
    {
        1 => "Initiated (Title Pending)",
        2 => "Title Verified",
        3 => "Deed Signed",
        4 => "Settled (Sold)",
        5 => "Cancelled",
        _ => $"Status #{status}"
    };
}
