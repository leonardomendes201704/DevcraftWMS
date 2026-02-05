using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundOrderNotificationConfiguration : IEntityTypeConfiguration<OutboundOrderNotification>
{
    public void Configure(EntityTypeBuilder<OutboundOrderNotification> builder)
    {
        builder.ToTable("OutboundOrderNotifications");

        builder.Property(x => x.EventType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasMaxLength(200);

        builder.Property(x => x.ToAddress)
            .HasMaxLength(200);

        builder.Property(x => x.ExternalId)
            .HasMaxLength(200);

        builder.HasOne(x => x.OutboundOrder)
            .WithMany()
            .HasForeignKey(x => x.OutboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CreatedAtUtc);
        builder.HasIndex(x => new { x.OutboundOrderId, x.Channel, x.EventType }).IsUnique();
        builder.HasIndex(x => x.Status);
    }
}
