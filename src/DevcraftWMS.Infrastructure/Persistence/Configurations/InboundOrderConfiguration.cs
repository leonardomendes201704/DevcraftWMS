using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class InboundOrderConfiguration : IEntityTypeConfiguration<InboundOrder>
{
    public void Configure(EntityTypeBuilder<InboundOrder> builder)
    {
        builder.ToTable("InboundOrders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderNumber).HasMaxLength(32).IsRequired();
        builder.Property(o => o.SupplierName).HasMaxLength(120);
        builder.Property(o => o.DocumentNumber).HasMaxLength(64);
        builder.Property(o => o.Notes).HasMaxLength(500);
        builder.Property(o => o.SuggestedDock).HasMaxLength(32);
        builder.Property(o => o.CancelReason).HasMaxLength(500);

        builder.HasIndex(o => new { o.CustomerId, o.OrderNumber }).IsUnique();
        builder.HasIndex(o => o.AsnId).IsUnique();
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.Priority);

        builder.HasOne(o => o.Asn)
            .WithMany()
            .HasForeignKey(o => o.AsnId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Warehouse)
            .WithMany()
            .HasForeignKey(o => o.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.InboundOrder)
            .HasForeignKey(i => i.InboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
