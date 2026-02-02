using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class AsnConfiguration : IEntityTypeConfiguration<Asn>
{
    public void Configure(EntityTypeBuilder<Asn> builder)
    {
        builder.ToTable("Asns");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.AsnNumber).HasMaxLength(32).IsRequired();
        builder.Property(a => a.SupplierName).HasMaxLength(120);
        builder.Property(a => a.DocumentNumber).HasMaxLength(64);
        builder.Property(a => a.Notes).HasMaxLength(500);

        builder.HasIndex(a => new { a.CustomerId, a.AsnNumber }).IsUnique();

        builder.HasMany(a => a.Items)
            .WithOne(i => i.Asn)
            .HasForeignKey(i => i.AsnId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
