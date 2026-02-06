using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class DockScheduleConfiguration : IEntityTypeConfiguration<DockSchedule>
{
    public void Configure(EntityTypeBuilder<DockSchedule> builder)
    {
        builder.ToTable("DockSchedules");

        builder.Property(x => x.DockCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.RescheduleReason)
            .HasMaxLength(300);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OutboundOrder)
            .WithMany()
            .HasForeignKey(x => x.OutboundOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.OutboundShipment)
            .WithMany()
            .HasForeignKey(x => x.OutboundShipmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => x.DockCode);
        builder.HasIndex(x => x.Status);
    }
}
