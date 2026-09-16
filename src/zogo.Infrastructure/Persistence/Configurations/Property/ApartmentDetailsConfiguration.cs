using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Configurations.Property;

public sealed class ApartmentDetailsConfiguration
    : IEntityTypeConfiguration<ApartmentDetails>
{
    public void Configure(EntityTypeBuilder<ApartmentDetails> builder)
    {
        builder.ToTable("ApartmentDetails");

        builder.HasKey(x => x.PropertyId);

        builder.Property(x => x.PropertyId)
            .HasColumnName("PropertyId")
            .IsRequired();

        builder.Property(x => x.CondominiumId)
            .HasColumnName("CondominiumId");

        builder.Property(x => x.ApartmentTypeId)
            .HasColumnName("ApartmentTypeId");

        builder.Property(x => x.FloorNumber)
            .HasColumnName("FloorNumber");

        builder.Property(x => x.UnitNumber)
            .HasColumnName("UnitNumber")
            .HasMaxLength(50);

        builder.Property(x => x.IsCornerUnit)
            .HasColumnName("IsCornerUnit")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.FloorAreaSqFt)
            .HasColumnName("FloorAreaSqFt")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Bedrooms)
            .HasColumnName("Bedrooms")
            .HasDefaultValue((short)0)
            .IsRequired();

        builder.Property(x => x.MasterBedrooms)
            .HasColumnName("MasterBedrooms")
            .HasDefaultValue((short)0)
            .IsRequired();

        builder.Property(x => x.Bathrooms)
            .HasColumnName("Bathrooms")
            .HasDefaultValue((short)0)
            .IsRequired();

        builder.Property(x => x.AttachedBathrooms)
            .HasColumnName("AttachedBathrooms")
            .HasDefaultValue((short)0)
            .IsRequired();

        builder.Property(x => x.Balconies)
            .HasColumnName("Balconies")
            .HasDefaultValue((short)0)
            .IsRequired();

        builder.Property(x => x.FurnishingTypeId)
            .HasColumnName("FurnishingTypeId");

        builder.Property(x => x.ViewTypeId)
            .HasColumnName("ViewTypeId");

        builder.Property(x => x.HasMaidRoom)
            .HasColumnName("HasMaidRoom")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.HasMaidBathroom)
            .HasColumnName("HasMaidBathroom")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.HasLaundryArea)
            .HasColumnName("HasLaundryArea")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.HasStorageRoom)
            .HasColumnName("HasStorageRoom")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.HasWalkInCloset)
            .HasColumnName("HasWalkInCloset")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.HasDriversRoom)
            .HasColumnName("HasDriversRoom")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasOne<Domain.Entities.Property.Property>()
            .WithOne()
            .HasForeignKey<ApartmentDetails>(x => x.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Condominium>()
            .WithMany()
            .HasForeignKey(x => x.CondominiumId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ApartmentType>()
            .WithMany()
            .HasForeignKey(x => x.ApartmentTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<FurnishingType>()
            .WithMany()
            .HasForeignKey(x => x.FurnishingTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ViewType>()
            .WithMany()
            .HasForeignKey(x => x.ViewTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}