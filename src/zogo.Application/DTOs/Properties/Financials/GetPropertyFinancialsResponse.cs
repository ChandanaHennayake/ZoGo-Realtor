namespace zogo.Application.DTOs.Property.Financials;

public sealed class GetPropertyFinancialsResponse
{
    public Guid PropertyId { get; init; }
    public decimal? MaintenanceFee { get; init; }
    public short? MaintenanceFeePeriod { get; init; }
    public decimal? SinkingFundAmount { get; init; }
    public short? SinkingFundPeriod { get; init; }
    public bool? BillsUpToDate { get; init; }
    public bool HasOutstandingCharges { get; init; }
    public decimal? OutstandingAmount { get; init; }
    public string? OutstandingDescription { get; init; }
}