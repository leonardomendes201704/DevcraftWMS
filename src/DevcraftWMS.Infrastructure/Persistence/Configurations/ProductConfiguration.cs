using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : AuditableEntityConfiguration<Product>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Code).HasMaxLength(32).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.Ean).HasMaxLength(50);
        builder.Property(p => p.ErpCode).HasMaxLength(50);
        builder.Property(p => p.Category).HasMaxLength(100);
        builder.Property(p => p.Brand).HasMaxLength(100);
        builder.Property(p => p.WeightKg).HasPrecision(18, 4);
        builder.Property(p => p.LengthCm).HasPrecision(18, 4);
        builder.Property(p => p.WidthCm).HasPrecision(18, 4);
        builder.Property(p => p.HeightCm).HasPrecision(18, 4);
        builder.Property(p => p.VolumeCm3).HasPrecision(18, 4);

        builder.HasOne(p => p.BaseUom)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.BaseUomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.Ean).IsUnique();
        builder.HasIndex(p => p.ErpCode).IsUnique();
    }
}
