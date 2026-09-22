using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence.Configurations.Master;

public sealed class DivisionalSecretariatConfiguration : IEntityTypeConfiguration<DivisionalSecretariat>
{
    public void Configure(EntityTypeBuilder<DivisionalSecretariat> builder)
    {
        builder.ToTable("DivisionalSecretariats");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.DistrictId)
            .HasColumnName("DistrictId")
            .IsRequired();

        builder.Property(x => x.Code)
            .HasColumnName("Code")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .IsRequired();
    }
}
