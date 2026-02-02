using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("Zones");

        builder.HasKey(z => z.Id);
        builder.Property(z => z.WarehouseId).IsRequired();
        builder.Property(z => z.Code).HasMaxLength(32).IsRequired();
        builder.Property(z => z.Name).HasMaxLength(200).IsRequired();
        builder.Property(z => z.Description).HasMaxLength(500);
        builder.Property(z => z.ZoneType).IsRequired();

        builder.HasOne(z => z.Warehouse)
            .WithMany(w => w.Zones)
            .HasForeignKey(z => z.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(z => new { z.WarehouseId, z.Code }).IsUnique();
        builder.HasIndex(z => z.WarehouseId);
    }
}
