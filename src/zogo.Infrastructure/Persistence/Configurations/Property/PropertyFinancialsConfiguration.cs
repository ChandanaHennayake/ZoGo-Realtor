using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Configurations.Property;

public sealed class PropertyFinancialsConfiguration
    : IEntityTypeConfiguration<PropertyFinancials>
{
    public void Configure(EntityTypeBuilder<PropertyFinancials> builder)
    {
        builder.ToTable("PropertyFinancials");

        builder.HasKey(x => x.PropertyId);

        builder.Property(x => x.PropertyId)
            .HasColumnName("PropertyId")
            .IsRequired();

        builder.Property(x => x.MaintenanceFee)
            .HasColumnName("MaintenanceFee")
            .HasPrecision(18, 2);

        builder.Property(x => x.MaintenanceFeePeriod)
            .HasColumnName("MaintenanceFeePeriod");

        builder.Property(x => x.SinkingFundAmount)
            .HasColumnName("SinkingFundAmount")
            .HasPrecision(18, 2);

        builder.Property(x => x.SinkingFundPeriod)
            .HasColumnName("SinkingFundPeriod");

        builder.Property(x => x.BillsUpToDate)
            .HasColumnName("BillsUpToDate");

        builder.Property(x => x.HasOutstandingCharges)
            .HasColumnName("HasOutstandingCharges")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.OutstandingAmount)
            .HasColumnName("OutstandingAmount")
            .HasPrecision(18, 2);

        builder.Property(x => x.OutstandingDescription)
            .HasColumnName("OutstandingDescription");

        
        builder.HasOne <Domain.Entities.Master.Property > ()
            .WithOne()
            .HasForeignKey<PropertyFinancials>(x => x.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}