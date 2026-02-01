using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Code).HasMaxLength(32).IsRequired();
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(1000);

        builder.HasOne(s => s.Sector)
            .WithMany(s => s.Sections)
            .HasForeignKey(s => s.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.SectorId, s.Code }).IsUnique();
    }
}
