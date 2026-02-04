using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class InboundOrderNotificationConfiguration : IEntityTypeConfiguration<InboundOrderNotification>
{
    public void Configure(EntityTypeBuilder<InboundOrderNotification> builder)
    {
        builder.ToTable("InboundOrderNotifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ToAddress)
            .HasMaxLength(320);

        builder.Property(x => x.Subject)
            .HasMaxLength(200);

        builder.Property(x => x.ExternalId)
            .HasMaxLength(64);

        builder.Property(x => x.LastError)
            .HasMaxLength(400);

        builder.HasIndex(x => new { x.InboundOrderId, x.Channel, x.EventType });
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
