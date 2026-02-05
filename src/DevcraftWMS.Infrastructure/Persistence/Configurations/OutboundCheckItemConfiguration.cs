using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundCheckItemConfiguration : IEntityTypeConfiguration<OutboundCheckItem>
{
    public void Configure(EntityTypeBuilder<OutboundCheckItem> builder)
    {
        builder.ToTable("OutboundCheckItems");

        builder.HasOne(x => x.OutboundOrderItem)
            .WithMany()
            .HasForeignKey(x => x.OutboundOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Uom)
            .WithMany()
            .HasForeignKey(x => x.UomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Evidence)
            .WithOne(e => e.OutboundCheckItem)
            .HasForeignKey(e => e.OutboundCheckItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OutboundCheckId);
    }
}
