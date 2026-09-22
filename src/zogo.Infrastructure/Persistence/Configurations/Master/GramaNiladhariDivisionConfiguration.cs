using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence.Configurations.Master;

public sealed class GramaNiladhariDivisionConfiguration : IEntityTypeConfiguration<GramaNiladhariDivision>
{
    public void Configure(EntityTypeBuilder<GramaNiladhariDivision> builder)
    {
        builder.ToTable("GramaNiladhariDivisions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.DivisionalSecretariatId)
            .HasColumnName("DivisionalSecretariatId")
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
