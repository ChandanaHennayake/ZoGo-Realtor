using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Buyer;

namespace zogo.Infrastructure.Persistence.Configurations.Buyer;

public class BuyerPropertyInteractionConfiguration : IEntityTypeConfiguration<BuyerPropertyInteraction>
{
    public void Configure(EntityTypeBuilder<BuyerPropertyInteraction> builder)
    {
        builder.ToTable("BuyerPropertyInteractions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(x => x.PropertyId)
            .HasColumnName("PropertyId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.BuyerId)
            .HasColumnName("BuyerId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.CurrentStatus)
            .HasColumnName("CurrentStatus")
            .HasColumnType("smallint")
            .IsRequired();

        builder.HasIndex(x => new { x.BuyerId, x.PropertyId })
            .IsUnique();

        builder.HasIndex(x => x.PropertyId);

        builder.HasOne(x => x.Property)
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Buyer)
            .WithMany()
            .HasForeignKey(x => x.BuyerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
