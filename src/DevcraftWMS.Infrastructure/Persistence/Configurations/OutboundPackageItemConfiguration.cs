using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundPackageItemConfiguration : IEntityTypeConfiguration<OutboundPackageItem>
{
    public void Configure(EntityTypeBuilder<OutboundPackageItem> builder)
    {
        builder.ToTable("OutboundPackageItems");

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

        builder.HasIndex(x => x.OutboundPackageId);
    }
}
