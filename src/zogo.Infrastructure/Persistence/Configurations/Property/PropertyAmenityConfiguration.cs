using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence.Configurations.Property;

public class PropertyAmenityConfiguration
    : IEntityTypeConfiguration<PropertyAmenity>
{
    public void Configure(EntityTypeBuilder<PropertyAmenity> builder)
    {
        builder.ToTable("PropertyAmenities");

        builder.HasKey(x => new
        {
            x.PropertyId,
            x.AmenityId
        });

        builder.Property(x => x.PropertyId)
            .HasColumnName("PropertyId")
            .IsRequired();

        builder.Property(x => x.AmenityId)
            .HasColumnName("AmenityId")
            .IsRequired();

        builder.HasOne(x => x.Amenity)
            .WithMany()
            .HasForeignKey(x => x.AmenityId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}