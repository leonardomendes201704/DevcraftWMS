using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class WarehouseContactConfiguration : AuditableEntityConfiguration<WarehouseContact>
{
    protected override void ConfigureEntity(EntityTypeBuilder<WarehouseContact> builder)
    {
        builder.ToTable("WarehouseContacts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ContactName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ContactEmail).HasMaxLength(200);
        builder.Property(x => x.ContactPhone).HasMaxLength(50);
        builder.HasIndex(x => new { x.WarehouseId, x.IsPrimary });
    }
}
