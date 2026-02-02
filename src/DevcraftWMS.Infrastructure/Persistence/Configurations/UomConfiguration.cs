using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class UomConfiguration : AuditableEntityConfiguration<Uom>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Uom> builder)
    {
        builder.ToTable("Uoms");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Code).HasMaxLength(16).IsRequired();
        builder.Property(u => u.Name).HasMaxLength(200).IsRequired();

        builder.HasIndex(u => u.Code).IsUnique();
    }
}
