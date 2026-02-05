using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class PickingTaskItemConfiguration : IEntityTypeConfiguration<PickingTaskItem>
{
    public void Configure(EntityTypeBuilder<PickingTaskItem> builder)
    {
        builder.ToTable("PickingTaskItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.QuantityPlanned).HasPrecision(18, 3);
        builder.Property(i => i.QuantityPicked).HasPrecision(18, 3);

        builder.HasIndex(i => i.OutboundOrderItemId);

        builder.HasOne(i => i.OutboundOrderItem)
            .WithMany()
            .HasForeignKey(i => i.OutboundOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Uom)
            .WithMany()
            .HasForeignKey(i => i.UomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Lot)
            .WithMany()
            .HasForeignKey(i => i.LotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Location)
            .WithMany()
            .HasForeignKey(i => i.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
