using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Code).HasMaxLength(32).IsRequired();
        builder.Property(l => l.Barcode).HasMaxLength(64).IsRequired();
        builder.Property(l => l.Level).IsRequired();
        builder.Property(l => l.Row).IsRequired();
        builder.Property(l => l.Column).IsRequired();

        builder.HasOne(l => l.Structure)
            .WithMany(s => s.Locations)
            .HasForeignKey(l => l.StructureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Zone)
            .WithMany(z => z.Locations)
            .HasForeignKey(l => l.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.StructureId, l.Code }).IsUnique();
    }
}
