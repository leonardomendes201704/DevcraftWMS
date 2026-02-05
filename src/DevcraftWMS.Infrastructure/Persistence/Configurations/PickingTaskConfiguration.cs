using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class PickingTaskConfiguration : IEntityTypeConfiguration<PickingTask>
{
    public void Configure(EntityTypeBuilder<PickingTask> builder)
    {
        builder.ToTable("PickingTasks");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Notes).HasMaxLength(500);

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => new { t.OutboundOrderId, t.Sequence });

        builder.HasOne(t => t.OutboundOrder)
            .WithMany(o => o.PickingTasks)
            .HasForeignKey(t => t.OutboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Warehouse)
            .WithMany()
            .HasForeignKey(t => t.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Items)
            .WithOne(i => i.PickingTask)
            .HasForeignKey(i => i.PickingTaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
