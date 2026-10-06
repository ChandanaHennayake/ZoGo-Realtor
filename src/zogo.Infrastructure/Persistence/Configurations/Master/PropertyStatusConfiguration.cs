using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence.Configurations.Master;

public sealed class PropertyStatusConfiguration : IEntityTypeConfiguration<PropertyStatus>
{
    public void Configure(EntityTypeBuilder<PropertyStatus> builder)
    {
        builder.ToTable("PropertyStatuses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasData(
            new PropertyStatus { Id = 1, Code = "DRAFT", Name = "Draft", IsActive = true },
            new PropertyStatus { Id = 2, Code = "PUBLISHED", Name = "Published", IsActive = true },
            new PropertyStatus { Id = 3, Code = "UNDER_REVIEW", Name = "Under Review", IsActive = true },
            new PropertyStatus { Id = 4, Code = "PENDING_APPROVAL", Name = "Pending Approval", IsActive = true },
            new PropertyStatus { Id = 5, Code = "SOLD", Name = "Sold", IsActive = true },
            new PropertyStatus { Id = 6, Code = "RENTED", Name = "Rented", IsActive = true },
            new PropertyStatus { Id = 7, Code = "SUSPENDED", Name = "Suspended", IsActive = true },
            new PropertyStatus { Id = 8, Code = "INACTIVE", Name = "Inactive", IsActive = true }
        );
    }
}
