using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zogo.Domain.Entities.Notification;

namespace zogo.Infrastructure.Persistence.Configurations.Notification;

public sealed class AgentNotificationConfiguration : IEntityTypeConfiguration<AgentNotification>
{
    public void Configure(EntityTypeBuilder<AgentNotification> builder)
    {
        builder.ToTable("AgentNotifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(x => x.AgentProfileId)
            .HasColumnName("AgentProfileId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.PropertyId)
            .HasColumnName("PropertyId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.BuyerId)
            .HasColumnName("BuyerId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("Title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasColumnName("Message")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.IsRead)
            .HasColumnName("IsRead")
            .HasColumnType("boolean")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasIndex(x => x.AgentProfileId);
        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => new { x.AgentProfileId, x.PropertyId, x.BuyerId });
        builder.HasIndex(x => x.CreatedAt);

        builder.HasOne(x => x.AgentProfile)
            .WithMany()
            .HasForeignKey(x => x.AgentProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Property)
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Buyer)
            .WithMany()
            .HasForeignKey(x => x.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
