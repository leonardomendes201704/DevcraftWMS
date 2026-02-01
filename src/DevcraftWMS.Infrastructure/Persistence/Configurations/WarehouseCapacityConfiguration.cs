using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class WarehouseCapacityConfiguration : AuditableEntityConfiguration<WarehouseCapacity>
{
    protected override void ConfigureEntity(EntityTypeBuilder<WarehouseCapacity> builder)
    {
        builder.ToTable("WarehouseCapacities");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LengthMeters).HasPrecision(18, 4);
        builder.Property(x => x.WidthMeters).HasPrecision(18, 4);
        builder.Property(x => x.HeightMeters).HasPrecision(18, 4);
        builder.Property(x => x.TotalAreaM2).HasPrecision(18, 4);
        builder.Property(x => x.TotalCapacity).HasPrecision(18, 4);
        builder.Property(x => x.MaxWeightKg).HasPrecision(18, 4);
        builder.Property(x => x.OperationalArea).HasPrecision(18, 4);
        builder.HasIndex(x => new { x.WarehouseId, x.IsPrimary });
    }
}
