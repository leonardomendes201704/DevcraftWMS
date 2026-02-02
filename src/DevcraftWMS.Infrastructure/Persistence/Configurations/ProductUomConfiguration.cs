using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ProductUomConfiguration : AuditableEntityConfiguration<ProductUom>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ProductUom> builder)
    {
        builder.ToTable("ProductUoms");

        builder.HasKey(pu => pu.Id);
        builder.Property(pu => pu.CustomerId).IsRequired();
        builder.HasIndex(pu => pu.CustomerId);
        builder.Property(pu => pu.ConversionFactor).HasPrecision(18, 4);

        builder.HasOne(pu => pu.Product)
            .WithMany(p => p.ProductUoms)
            .HasForeignKey(pu => pu.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pu => pu.Uom)
            .WithMany(u => u.ProductUoms)
            .HasForeignKey(pu => pu.UomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pu => new { pu.CustomerId, pu.ProductId, pu.UomId }).IsUnique();
        builder.HasIndex(pu => new { pu.ProductId, pu.IsBase });
    }
}
