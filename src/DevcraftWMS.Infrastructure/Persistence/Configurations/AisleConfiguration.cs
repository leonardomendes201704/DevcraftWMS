using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class AisleConfiguration : IEntityTypeConfiguration<Aisle>
{
    public void Configure(EntityTypeBuilder<Aisle> builder)
    {
        builder.ToTable("Aisles");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Code).HasMaxLength(32).IsRequired();
        builder.Property(a => a.Name).HasMaxLength(200).IsRequired();

        builder.HasOne(a => a.Section)
            .WithMany(s => s.Aisles)
            .HasForeignKey(a => a.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.SectionId, a.Code }).IsUnique();
    }
}
