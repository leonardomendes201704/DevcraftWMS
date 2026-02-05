using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundShipmentConfiguration : IEntityTypeConfiguration<OutboundShipment>
{
    public void Configure(EntityTypeBuilder<OutboundShipment> builder)
    {
        builder.ToTable("OutboundShipments");

        builder.Property(x => x.DockCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.OutboundOrder)
            .WithMany(o => o.OutboundShipments)
            .HasForeignKey(x => x.OutboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(i => i.OutboundShipment)
            .HasForeignKey(i => i.OutboundShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OutboundOrderId);
    }
}
