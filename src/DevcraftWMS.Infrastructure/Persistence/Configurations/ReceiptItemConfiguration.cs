using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ReceiptItemConfiguration : AuditableEntityConfiguration<ReceiptItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ReceiptItem> builder)
    {
        builder.ToTable("ReceiptItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ReceiptId).IsRequired();
        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.LotId);
        builder.Property(i => i.LocationId).IsRequired();
        builder.Property(i => i.UomId).IsRequired();
        builder.Property(i => i.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();
        builder.Property(i => i.UnitCost)
            .HasPrecision(18, 4);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Lot)
            .WithMany()
            .HasForeignKey(i => i.LotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Location)
            .WithMany()
            .HasForeignKey(i => i.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Uom)
            .WithMany()
            .HasForeignKey(i => i.UomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.ReceiptId);
        builder.HasIndex(i => i.ProductId);
        builder.HasIndex(i => i.LocationId);
        builder.HasIndex(i => i.LotId);
    }
}
