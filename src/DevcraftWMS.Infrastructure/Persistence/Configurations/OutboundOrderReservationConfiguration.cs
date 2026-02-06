using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundOrderReservationConfiguration : IEntityTypeConfiguration<OutboundOrderReservation>
{
    public void Configure(EntityTypeBuilder<OutboundOrderReservation> builder)
    {
        builder.ToTable("OutboundOrderReservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityReserved)
            .HasPrecision(18, 4);

        builder.HasOne(x => x.OutboundOrder)
            .WithMany()
            .HasForeignKey(x => x.OutboundOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OutboundOrderItem)
            .WithMany()
            .HasForeignKey(x => x.OutboundOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InventoryBalance)
            .WithMany()
            .HasForeignKey(x => x.InventoryBalanceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.OutboundOrderId, x.OutboundOrderItemId });
        builder.HasIndex(x => x.InventoryBalanceId);
    }
}
