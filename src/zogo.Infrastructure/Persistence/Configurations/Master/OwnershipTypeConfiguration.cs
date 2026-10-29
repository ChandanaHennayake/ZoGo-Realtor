using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence.Configurations.Master;

public class OwnershipTypeConfiguration : IEntityTypeConfiguration<OwnershipType>
{
    public void Configure(EntityTypeBuilder<OwnershipType> builder)
    {
        builder.ToTable("OwnershipTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        builder.Property(x => x.Code)
            .HasColumnName("Code")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .HasColumnName("DisplayOrder")
            .HasDefaultValue(0)
            .IsRequired();
    }
}
