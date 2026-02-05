using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundOrderConfiguration : IEntityTypeConfiguration<OutboundOrder>
{
    public void Configure(EntityTypeBuilder<OutboundOrder> builder)
    {
        builder.ToTable("OutboundOrders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderNumber).HasMaxLength(32).IsRequired();
        builder.Property(o => o.CustomerReference).HasMaxLength(64);
        builder.Property(o => o.CarrierName).HasMaxLength(120);
        builder.Property(o => o.Notes).HasMaxLength(500);
        builder.Property(o => o.CancelReason).HasMaxLength(500);

        builder.HasIndex(o => new { o.CustomerId, o.OrderNumber }).IsUnique();
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.Priority);

        builder.HasOne(o => o.Warehouse)
            .WithMany()
            .HasForeignKey(o => o.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.OutboundOrder)
            .HasForeignKey(i => i.OutboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
