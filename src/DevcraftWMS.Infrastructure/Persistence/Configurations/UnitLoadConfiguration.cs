using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class UnitLoadConfiguration : AuditableEntityConfiguration<UnitLoad>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UnitLoad> builder)
    {
        builder.ToTable("UnitLoads");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.CustomerId).IsRequired();
        builder.Property(u => u.WarehouseId).IsRequired();
        builder.Property(u => u.ReceiptId).IsRequired();
        builder.Property(u => u.SsccInternal)
            .HasMaxLength(18)
            .IsRequired();
        builder.Property(u => u.SsccExternal)
            .HasMaxLength(50);
        builder.Property(u => u.Status).IsRequired();
        builder.Property(u => u.PrintedAtUtc);
        builder.Property(u => u.Notes)
            .HasMaxLength(500);

        builder.HasOne(u => u.Warehouse)
            .WithMany()
            .HasForeignKey(u => u.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Receipt)
            .WithMany()
            .HasForeignKey(u => u.ReceiptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.CustomerId);
        builder.HasIndex(u => u.WarehouseId);
        builder.HasIndex(u => u.ReceiptId);
        builder.HasIndex(u => u.SsccInternal).IsUnique();
        builder.HasIndex(u => u.Status);
        builder.HasIndex(u => u.PrintedAtUtc);
    }
}
