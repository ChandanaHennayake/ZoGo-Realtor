using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Configurations.Property;

public class PropertyMediaConfiguration
    : IEntityTypeConfiguration<PropertyMedia>
{
    public void Configure(EntityTypeBuilder<PropertyMedia> builder)
    {
        builder.ToTable("PropertyMedia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(x => x.PropertyId)
            .HasColumnName("PropertyId")
            .IsRequired();

        builder.Property(x => x.MediaType)
            .HasColumnName("MediaType")
            .IsRequired();

        builder.Property(x => x.StorageKey)
            .HasColumnName("StorageKey")
            .IsRequired();

        builder.Property(x => x.OriginalFileName)
            .HasColumnName("OriginalFileName")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.MimeType)
            .HasColumnName("MimeType")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FileSizeBytes)
            .HasColumnName("FileSizeBytes")
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .HasColumnName("DisplayOrder")
            .IsRequired();

        builder.Property(x => x.IsCover)
            .HasColumnName("IsCover")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasColumnName("CreatedBy")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.HasOne<Domain.Entities.Master.Property>()
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}