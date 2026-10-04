using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Configurations.Property;

public class PropertyDocumentConfiguration : IEntityTypeConfiguration<PropertyDocument>
{
    public void Configure(EntityTypeBuilder<PropertyDocument> builder)
    {
        builder.ToTable("PropertyDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(x => x.PropertyId)
            .HasColumnName("PropertyId")
            .IsRequired();

        builder.Property(x => x.DocumentTypeId)
            .HasColumnName("DocumentTypeId")
            .IsRequired();

        builder.Property(x => x.StorageKey)
            .HasColumnName("StorageKey")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.OriginalFileName)
            .HasColumnName("OriginalFileName")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.MimeType)
            .HasColumnName("MimeType")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.FileSizeBytes)
            .HasColumnName("FileSizeBytes")
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasColumnName("Status")
            .HasColumnType("smallint")
            .HasDefaultValue((short)1)
            .IsRequired();

        builder.Property(x => x.UploadedBy)
            .HasColumnName("UploadedBy")
            .IsRequired();

        builder.Property(x => x.UploadedAt)
            .HasColumnName("UploadedAt")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(x => x.VerifiedBy)
            .HasColumnName("VerifiedBy")
            .IsRequired(false);

        builder.Property(x => x.VerifiedAt)
            .HasColumnName("VerifiedAt")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.RejectionReason)
            .HasColumnName("RejectionReason")
            .HasColumnType("text")
            .IsRequired(false);

        builder.HasOne<Domain.Entities.Master.Property>()
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
