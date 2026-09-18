using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Configurations.Property;

public sealed class PropertyLegalDetailsConfiguration
    : IEntityTypeConfiguration<PropertyLegalDetails>
{
    public void Configure(EntityTypeBuilder<PropertyLegalDetails> builder)
    {
        builder.ToTable("PropertyLegalDetails");

        builder.HasKey(x => x.PropertyId);

        builder.Property(x => x.PropertyId)
            .HasColumnName("PropertyId")
            .IsRequired();

        builder.Property(x => x.OwnershipType)
            .HasColumnName("OwnershipType");

        builder.Property(x => x.HasMortgage)
            .HasColumnName("HasMortgage")
            .IsRequired();

        builder.Property(x => x.MortgageProvider)
            .HasColumnName("MortgageProvider")
            .HasMaxLength(200);

        builder.Property(x => x.HasLegalIssues)
            .HasColumnName("HasLegalIssues")
            .IsRequired();

        builder.Property(x => x.LegalIssueDescription)
            .HasColumnName("LegalIssueDescription");

        builder.Property(x => x.LegalVerified)
            .HasColumnName("LegalVerified")
            .IsRequired();

        builder.Property(x => x.VerifiedBy)
            .HasColumnName("VerifiedBy");

        builder.Property(x => x.VerifiedAt)
            .HasColumnName("VerifiedAt");

        // PropertyId -> Properties.Id
        builder.HasOne<Domain.Entities.Master.Property>()
            .WithOne()
            .HasForeignKey<PropertyLegalDetails>(x => x.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}