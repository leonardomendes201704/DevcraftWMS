using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class WarehouseAddressConfiguration : AuditableEntityConfiguration<WarehouseAddress>
{
    protected override void ConfigureEntity(EntityTypeBuilder<WarehouseAddress> builder)
    {
        builder.ToTable("WarehouseAddresses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AddressLine1).HasMaxLength(200).IsRequired();
        builder.Property(x => x.AddressNumber).HasMaxLength(20);
        builder.Property(x => x.AddressLine2).HasMaxLength(200);
        builder.Property(x => x.District).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100).IsRequired();
        builder.Property(x => x.State).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PostalCode).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Country).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Latitude).HasPrecision(9, 6);
        builder.Property(x => x.Longitude).HasPrecision(9, 6);
        builder.HasIndex(x => new { x.WarehouseId, x.IsPrimary });
    }
}
