using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ReceiptConfiguration : AuditableEntityConfiguration<Receipt>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Receipt> builder)
    {
        builder.ToTable("Receipts");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.CustomerId).IsRequired();
        builder.Property(r => r.WarehouseId).IsRequired();
        builder.Property(r => r.ReceiptNumber)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(r => r.DocumentNumber)
            .HasMaxLength(50);
        builder.Property(r => r.SupplierName)
            .HasMaxLength(120);
        builder.Property(r => r.Status)
            .IsRequired();
        builder.Property(r => r.ReceivedAtUtc);
        builder.Property(r => r.Notes)
            .HasMaxLength(500);

        builder.HasOne(r => r.Warehouse)
            .WithMany()
            .HasForeignKey(r => r.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Items)
            .WithOne(i => i.Receipt)
            .HasForeignKey(i => i.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.CustomerId);
        builder.HasIndex(r => r.WarehouseId);
        builder.HasIndex(r => r.ReceiptNumber);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.ReceivedAtUtc);
    }
}
