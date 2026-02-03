using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class GateCheckinConfiguration : AuditableEntityConfiguration<GateCheckin>
{
    protected override void ConfigureEntity(EntityTypeBuilder<GateCheckin> builder)
    {
        builder.ToTable("GateCheckins");
        builder.HasKey(gc => gc.Id);

        builder.Property(gc => gc.CustomerId).IsRequired();
        builder.Property(gc => gc.InboundOrderId);
        builder.Property(gc => gc.DocumentNumber)
            .HasMaxLength(64);
        builder.Property(gc => gc.VehiclePlate)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(gc => gc.DriverName)
            .HasMaxLength(120)
            .IsRequired();
        builder.Property(gc => gc.CarrierName)
            .HasMaxLength(120);
        builder.Property(gc => gc.ArrivalAtUtc)
            .IsRequired();
        builder.Property(gc => gc.Status)
            .IsRequired();
        builder.Property(gc => gc.Notes)
            .HasMaxLength(500);

        builder.HasOne(gc => gc.InboundOrder)
            .WithMany()
            .HasForeignKey(gc => gc.InboundOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(gc => gc.CustomerId);
        builder.HasIndex(gc => gc.InboundOrderId);
        builder.HasIndex(gc => gc.DocumentNumber);
        builder.HasIndex(gc => gc.VehiclePlate);
        builder.HasIndex(gc => gc.Status);
        builder.HasIndex(gc => gc.ArrivalAtUtc);
    }
}
