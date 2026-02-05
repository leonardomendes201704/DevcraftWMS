using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundOrderItemConfiguration : IEntityTypeConfiguration<OutboundOrderItem>
{
    public void Configure(EntityTypeBuilder<OutboundOrderItem> builder)
    {
        builder.ToTable("OutboundOrderItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Quantity).HasPrecision(18, 3);
        builder.Property(i => i.LotCode).HasMaxLength(64);

        builder.HasIndex(i => i.OutboundOrderId);
        builder.HasIndex(i => i.ProductId);
        builder.HasIndex(i => i.UomId);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Uom)
            .WithMany()
            .HasForeignKey(i => i.UomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
