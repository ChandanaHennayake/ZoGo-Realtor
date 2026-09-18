using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence.Configurations.Master;

public sealed class CondominiumConfiguration
    : IEntityTypeConfiguration<Condominium>
{
    public void Configure(EntityTypeBuilder<Condominium> builder)
    {
        builder.ToTable("Condominiums");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasColumnName("Address");

        builder.Property(x => x.DistrictId)
            .HasColumnName("DistrictId");

        builder.Property(x => x.DeveloperName)
            .HasColumnName("DeveloperName")
            .HasMaxLength(200);

        builder.Property(x => x.TotalFloors)
            .HasColumnName("TotalFloors");

        builder.Property(x => x.Latitude)
            .HasColumnName("Latitude")
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasColumnName("Longitude")
            .HasPrecision(9, 6);

        builder.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasOne<District>()
            .WithMany()
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}