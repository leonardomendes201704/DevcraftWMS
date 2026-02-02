using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class InventoryBalanceConfiguration : AuditableEntityConfiguration<InventoryBalance>
{
    protected override void ConfigureEntity(EntityTypeBuilder<InventoryBalance> builder)
    {
        builder.ToTable("InventoryBalances");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.LocationId).IsRequired();
        builder.Property(b => b.ProductId).IsRequired();
        builder.Property(b => b.LotId);
        builder.Property(b => b.QuantityOnHand).HasPrecision(18, 4).IsRequired();
        builder.Property(b => b.QuantityReserved).HasPrecision(18, 4).IsRequired();
        builder.Property(b => b.Status).IsRequired();

        builder.HasOne(b => b.Location)
            .WithMany()
            .HasForeignKey(b => b.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Product)
            .WithMany()
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Lot)
            .WithMany()
            .HasForeignKey(b => b.LotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.LocationId);
        builder.HasIndex(b => b.ProductId);
        builder.HasIndex(b => b.LotId);
        builder.HasIndex(b => new { b.LocationId, b.ProductId, b.LotId }).IsUnique();
    }
}
