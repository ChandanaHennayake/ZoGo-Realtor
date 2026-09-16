using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Configurations.Property;

public sealed class ApartmentTypeConfiguration
    : IEntityTypeConfiguration<ApartmentType>
{
    public void Configure(EntityTypeBuilder<ApartmentType> builder)
    {
        builder.ToTable("ApartmentTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Code)
            .HasColumnName("Code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true)
            .IsRequired();
    }
}