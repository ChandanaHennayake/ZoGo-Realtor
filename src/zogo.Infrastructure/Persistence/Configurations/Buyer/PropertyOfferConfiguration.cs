using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Buyer;

namespace zogo.Infrastructure.Persistence.Configurations.Buyer;

public sealed class PropertyOfferConfiguration : IEntityTypeConfiguration<PropertyOffer>
{
    public void Configure(EntityTypeBuilder<PropertyOffer> builder)
    {
        builder.ToTable("PropertyOffers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PropertyId)
            .IsRequired();

        builder.Property(x => x.BuyerId)
            .IsRequired();

        builder.Property(x => x.OfferAmount)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.CounterAmount)
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.BuyerNotes)
            .HasMaxLength(500);

        builder.Property(x => x.SellerNotes)
            .HasMaxLength(500);

        builder.Property(x => x.ExpiresAt);

        builder.Property(x => x.Status)
            .HasDefaultValue((short)1)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.BuyerId);
        builder.HasIndex(x => x.Status);

        builder.HasOne(x => x.Property)
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Buyer)
            .WithMany()
            .HasForeignKey(x => x.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
