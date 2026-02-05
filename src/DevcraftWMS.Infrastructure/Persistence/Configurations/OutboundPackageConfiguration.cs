using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundPackageConfiguration : IEntityTypeConfiguration<OutboundPackage>
{
    public void Configure(EntityTypeBuilder<OutboundPackage> builder)
    {
        builder.ToTable("OutboundPackages");

        builder.Property(x => x.PackageNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(x => x.Items)
            .WithOne(i => i.OutboundPackage)
            .HasForeignKey(i => i.OutboundPackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OutboundOrder)
            .WithMany(o => o.OutboundPackages)
            .HasForeignKey(x => x.OutboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OutboundOrderId);
        builder.HasIndex(x => new { x.OutboundOrderId, x.PackageNumber }).IsUnique();
    }
}
