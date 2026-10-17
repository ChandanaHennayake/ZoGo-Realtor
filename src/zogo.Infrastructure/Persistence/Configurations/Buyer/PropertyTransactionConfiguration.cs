using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Buyer;

namespace zogo.Infrastructure.Persistence.Configurations.Buyer;

public sealed class PropertyTransactionConfiguration : IEntityTypeConfiguration<PropertyTransaction>
{
    public void Configure(EntityTypeBuilder<PropertyTransaction> builder)
    {
        builder.ToTable("PropertyTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PropertyId)
            .IsRequired();

        builder.Property(x => x.OfferId);

        builder.Property(x => x.BuyerId)
            .IsRequired();

        builder.Property(x => x.SellerId)
            .IsRequired();

        builder.Property(x => x.AgentId);

        builder.Property(x => x.AgreedPrice)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.CommissionRate)
            .HasColumnType("numeric(5,4)")
            .IsRequired();

        builder.Property(x => x.CommissionAmount)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasDefaultValue((short)1)
            .IsRequired();

        builder.Property(x => x.TitleDeedReference)
            .HasMaxLength(100);

        builder.Property(x => x.NotaryName)
            .HasMaxLength(150);

        builder.Property(x => x.ClosingNotes)
            .HasMaxLength(1000);

        builder.Property(x => x.ClosingDate);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.BuyerId);
        builder.HasIndex(x => x.SellerId);
        builder.HasIndex(x => x.AgentId);
        builder.HasIndex(x => x.Status);

        builder.HasOne(x => x.Property)
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Offer)
            .WithMany()
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Buyer)
            .WithMany()
            .HasForeignKey(x => x.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Seller)
            .WithMany()
            .HasForeignKey(x => x.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Agent)
            .WithMany()
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
