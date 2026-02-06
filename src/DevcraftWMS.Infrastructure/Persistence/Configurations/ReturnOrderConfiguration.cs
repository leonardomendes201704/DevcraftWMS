using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ReturnOrderConfiguration : IEntityTypeConfiguration<ReturnOrder>
{
    public void Configure(EntityTypeBuilder<ReturnOrder> builder)
    {
        builder.ToTable("ReturnOrders");

        builder.Property(x => x.ReturnNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasMany(x => x.Items)
            .WithOne(i => i.ReturnOrder)
            .HasForeignKey(i => i.ReturnOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OutboundOrder)
            .WithMany()
            .HasForeignKey(x => x.OutboundOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ReturnNumber);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.WarehouseId);
    }
}
