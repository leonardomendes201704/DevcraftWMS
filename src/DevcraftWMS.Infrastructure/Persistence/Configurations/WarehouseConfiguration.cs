using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class WarehouseConfiguration : AuditableEntityConfiguration<Warehouse>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ShortName).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.ExternalId).HasMaxLength(100);
        builder.Property(x => x.ErpCode).HasMaxLength(100);
        builder.Property(x => x.CostCenterCode).HasMaxLength(50);
        builder.Property(x => x.CostCenterName).HasMaxLength(200);
        builder.Property(x => x.Timezone).HasMaxLength(100);

        builder.HasMany(x => x.Addresses)
            .WithOne(x => x.Warehouse)
            .HasForeignKey(x => x.WarehouseId);

        builder.HasMany(x => x.Contacts)
            .WithOne(x => x.Warehouse)
            .HasForeignKey(x => x.WarehouseId);

        builder.HasMany(x => x.Capacities)
            .WithOne(x => x.Warehouse)
            .HasForeignKey(x => x.WarehouseId);
    }
}
