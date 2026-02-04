using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class LotConfiguration : AuditableEntityConfiguration<Lot>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Lot> builder)
    {
        builder.ToTable("Lots");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.ProductId).IsRequired();
        builder.Property(l => l.Code)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(l => l.ManufactureDate);
        builder.Property(l => l.ExpirationDate);
        builder.Property(l => l.Status)
            .IsRequired();
        builder.Property(l => l.QuarantinedAtUtc);
        builder.Property(l => l.QuarantineReason)
            .HasMaxLength(200);

        builder.HasOne(l => l.Product)
            .WithMany(p => p.Lots)
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.ProductId, l.Code }).IsUnique();
        builder.HasIndex(l => l.ExpirationDate);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => l.ProductId);
    }
}
