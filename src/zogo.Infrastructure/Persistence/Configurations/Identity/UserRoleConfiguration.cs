using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Identity;

namespace zogo.Infrastructure.Persistence.Configurations.Identity;

public sealed class UserRoleConfiguration
    : IEntityTypeConfiguration<UserRole>
{
    public void Configure(
        EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        // Composite Primary Key
        builder.HasKey(x => new
        {
            x.UserId,
            x.RoleId
        });

        builder.Property(x => x.UserId)
            .HasColumnType("uuid");

        builder.Property(x => x.RoleId)
            .HasColumnType("smallint");

        builder.Property(x => x.AssignedAt)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()");

        builder.Property(x => x.AssignedBy)
            .HasColumnType("uuid");

        // User relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Role relationship
        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}