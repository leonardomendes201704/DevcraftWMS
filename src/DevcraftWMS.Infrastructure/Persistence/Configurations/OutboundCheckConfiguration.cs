using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundCheckConfiguration : IEntityTypeConfiguration<OutboundCheck>
{
    public void Configure(EntityTypeBuilder<OutboundCheck> builder)
    {
        builder.ToTable("OutboundChecks");

        builder.HasMany(x => x.Items)
            .WithOne(i => i.OutboundCheck)
            .HasForeignKey(i => i.OutboundCheckId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OutboundOrder)
            .WithMany(o => o.OutboundChecks)
            .HasForeignKey(x => x.OutboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OutboundOrderId);
    }
}
