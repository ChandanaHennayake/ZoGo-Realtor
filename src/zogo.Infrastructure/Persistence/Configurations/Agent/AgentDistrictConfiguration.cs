using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Agent;

namespace zogo.Infrastructure.Persistence.Configurations.Agent;

public sealed class AgentDistrictConfiguration : IEntityTypeConfiguration<AgentDistrict>
{
    public void Configure(EntityTypeBuilder<AgentDistrict> builder)
    {
        builder.ToTable("AgentDistricts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(x => x.AgentId)
            .HasColumnName("AgentId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.DistrictId)
            .HasColumnName("DistrictId")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasIndex(x => new { x.AgentId, x.DistrictId })
            .IsUnique();

        builder.HasIndex(x => x.AgentId);

        builder.HasIndex(x => x.DistrictId);

        builder.HasOne(x => x.AgentProfile)
            .WithMany(x => x.AgentDistricts)
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.District)
            .WithMany()
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
