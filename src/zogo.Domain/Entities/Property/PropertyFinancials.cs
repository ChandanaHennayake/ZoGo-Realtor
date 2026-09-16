namespace zogo.Domain.Entities.Property;

public class PropertyFinancials
{
    private PropertyFinancials()
    {
    }

    public Guid PropertyId { get; private set; }

    public decimal? MaintenanceFee { get; private set; }

    public short? MaintenanceFeePeriod { get; private set; }

    public decimal? SinkingFundAmount { get; private set; }

    public short? SinkingFundPeriod { get; private set; }

    public bool? BillsUpToDate { get; private set; }

    public bool HasOutstandingCharges { get; private set; }

    public decimal? OutstandingAmount { get; private set; }

    public string? OutstandingDescription { get; private set; }

    public static PropertyFinancials Create(
        Guid propertyId,
        decimal? maintenanceFee,
        short? maintenanceFeePeriod,
        decimal? sinkingFundAmount,
        short? sinkingFundPeriod,
        bool? billsUpToDate,
        bool hasOutstandingCharges,
        decimal? outstandingAmount,
        string? outstandingDescription)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        if (maintenanceFee.HasValue && maintenanceFee.Value < 0)
            throw new ArgumentException(
                "Maintenance fee cannot be negative.",
                nameof(maintenanceFee));

        if (sinkingFundAmount.HasValue && sinkingFundAmount.Value < 0)
            throw new ArgumentException(
                "Sinking fund amount cannot be negative.",
                nameof(sinkingFundAmount));

        if (outstandingAmount.HasValue && outstandingAmount.Value < 0)
            throw new ArgumentException(
                "Outstanding amount cannot be negative.",
                nameof(outstandingAmount));

        if (!hasOutstandingCharges && outstandingAmount.HasValue)
            throw new ArgumentException(
                "Outstanding amount cannot be provided when there are no outstanding charges.",
                nameof(outstandingAmount));

        return new PropertyFinancials
        {
            PropertyId = propertyId,
            MaintenanceFee = maintenanceFee,
            MaintenanceFeePeriod = maintenanceFeePeriod,
            SinkingFundAmount = sinkingFundAmount,
            SinkingFundPeriod = sinkingFundPeriod,
            BillsUpToDate = billsUpToDate,
            HasOutstandingCharges = hasOutstandingCharges,
            OutstandingAmount = outstandingAmount,
            OutstandingDescription =
                string.IsNullOrWhiteSpace(outstandingDescription)
                    ? null
                    : outstandingDescription.Trim()
        };
    }

    public void Update(
        decimal? maintenanceFee,
        short? maintenanceFeePeriod,
        decimal? sinkingFundAmount,
        short? sinkingFundPeriod,
        bool? billsUpToDate,
        bool hasOutstandingCharges,
        decimal? outstandingAmount,
        string? outstandingDescription)
    {
        if (maintenanceFee.HasValue && maintenanceFee.Value < 0)
            throw new ArgumentException(
                "Maintenance fee cannot be negative.",
                nameof(maintenanceFee));

        if (sinkingFundAmount.HasValue && sinkingFundAmount.Value < 0)
            throw new ArgumentException(
                "Sinking fund amount cannot be negative.",
                nameof(sinkingFundAmount));

        if (outstandingAmount.HasValue && outstandingAmount.Value < 0)
            throw new ArgumentException(
                "Outstanding amount cannot be negative.",
                nameof(outstandingAmount));

        if (!hasOutstandingCharges && outstandingAmount.HasValue)
            throw new ArgumentException(
                "Outstanding amount cannot be provided when there are no outstanding charges.",
                nameof(outstandingAmount));

        MaintenanceFee = maintenanceFee;
        MaintenanceFeePeriod = maintenanceFeePeriod;
        SinkingFundAmount = sinkingFundAmount;
        SinkingFundPeriod = sinkingFundPeriod;
        BillsUpToDate = billsUpToDate;
        HasOutstandingCharges = hasOutstandingCharges;
        OutstandingAmount = outstandingAmount;

        OutstandingDescription =
            string.IsNullOrWhiteSpace(outstandingDescription)
                ? null
                : outstandingDescription.Trim();
    }
}