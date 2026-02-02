using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class AsnItemConfiguration : IEntityTypeConfiguration<AsnItem>
{
    public void Configure(EntityTypeBuilder<AsnItem> builder)
    {
        builder.ToTable("AsnItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Quantity).HasPrecision(18, 3).IsRequired();
        builder.Property(i => i.LotCode).HasMaxLength(64);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Uom)
            .WithMany()
            .HasForeignKey(i => i.UomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
