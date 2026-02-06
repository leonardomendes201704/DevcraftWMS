using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class PickingReplenishmentTaskConfiguration : IEntityTypeConfiguration<PickingReplenishmentTask>
{
    public void Configure(EntityTypeBuilder<PickingReplenishmentTask> builder)
    {
        builder.ToTable("PickingReplenishmentTasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.QuantityPlanned)
            .HasPrecision(18, 4);

        builder.Property(t => t.QuantityMoved)
            .HasPrecision(18, 4);

        builder.Property(t => t.Notes)
            .HasMaxLength(500);

        builder.HasOne(t => t.Warehouse)
            .WithMany()
            .HasForeignKey(t => t.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Product)
            .WithMany()
            .HasForeignKey(t => t.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Uom)
            .WithMany()
            .HasForeignKey(t => t.UomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.FromLocation)
            .WithMany()
            .HasForeignKey(t => t.FromLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ToLocation)
            .WithMany()
            .HasForeignKey(t => t.ToLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => new { t.ProductId, t.ToLocationId, t.Status });
    }
}
