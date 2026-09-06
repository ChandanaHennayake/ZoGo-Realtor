using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Configurations.Property;

public sealed class PropertyConfiguration
    : IEntityTypeConfiguration<Domain.Entities.Property.Property>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Property.Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ReferenceNo)
            .HasColumnName("ReferenceNo")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.OwnerUserId)
            .HasColumnName("OwnerUserId")
            .IsRequired();

        builder.Property(x => x.PropertyTypeId)
            .HasColumnName("PropertyTypeId")
            .IsRequired();

        builder.Property(x => x.ListingTypeId)
            .HasColumnName("ListingTypeId")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("Title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("Description");

        builder.Property(x => x.DistrictId)
            .HasColumnName("DistrictId")
            .IsRequired();

        builder.Property(x => x.DivisionalSecretariatId)
            .HasColumnName("DivisionalSecretariatId");

        builder.Property(x => x.GnDivisionId)
            .HasColumnName("GnDivisionId");

        builder.Property(x => x.AddressLine1)
            .HasColumnName("AddressLine1")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.AddressLine2)
            .HasColumnName("AddressLine2")
            .HasMaxLength(250);

        builder.Property(x => x.City)
            .HasColumnName("City")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PostalCode)
            .HasColumnName("PostalCode")
            .HasMaxLength(20);

        builder.Property(x => x.Latitude)
            .HasColumnName("Latitude")
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasColumnName("Longitude")
            .HasPrecision(9, 6);

        builder.Property(x => x.AskingPrice)
            .HasColumnName("AskingPrice")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.IsNegotiable)
            .HasColumnName("IsNegotiable")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("Status")
            .HasDefaultValue((short)1)
            .IsRequired();

        builder.Property(x => x.PublishedAt)
            .HasColumnName("PublishedAt");

        builder.Property(x => x.SoldAt)
            .HasColumnName("SoldAt");

        builder.Property(x => x.CreatedBy)
            .HasColumnName("CreatedBy")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasColumnName("UpdatedBy");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(x => x.DeletedAt)
            .HasColumnName("DeletedAt");

        builder.HasIndex(x => x.ReferenceNo)
            .IsUnique()
            .HasDatabaseName("UX_Properties_ReferenceNo");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Properties_Status");

        builder.HasIndex(x => new
        {
            x.PropertyTypeId,
            x.DistrictId,
            x.Status,
            x.AskingPrice
        })
        .HasDatabaseName("IX_Properties_Search");

        builder.HasIndex(x => x.OwnerUserId)
            .HasDatabaseName("IX_Properties_Owner");

        builder.HasOne<Domain.Entities.Identity.User>()
            .WithMany()
            .HasForeignKey(x => x.OwnerUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<PropertyType>()
            .WithMany()
            .HasForeignKey(x => x.PropertyTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ListingType>()
            .WithMany()
            .HasForeignKey(x => x.ListingTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<District>()
            .WithMany()
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<DivisionalSecretariat>()
            .WithMany()
            .HasForeignKey(x => x.DivisionalSecretariatId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<GramaNiladhariDivision>()
            .WithMany()
            .HasForeignKey(x => x.GnDivisionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Domain.Entities.Identity.User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Domain.Entities.Identity.User>()
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}