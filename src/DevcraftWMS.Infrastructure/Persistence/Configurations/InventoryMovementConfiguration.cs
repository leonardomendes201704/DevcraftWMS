using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .HasMaxLength(200);

        builder.Property(x => x.Reference)
            .HasMaxLength(120);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FromLocation)
            .WithMany()
            .HasForeignKey(x => x.FromLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToLocation)
            .WithMany()
            .HasForeignKey(x => x.ToLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Lot)
            .WithMany()
            .HasForeignKey(x => x.LotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.FromLocationId);
        builder.HasIndex(x => x.ToLocationId);
        builder.HasIndex(x => x.PerformedAtUtc);
        builder.HasIndex(x => x.CustomerId);
    }
}
