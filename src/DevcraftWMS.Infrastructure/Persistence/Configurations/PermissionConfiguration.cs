using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : AuditableEntityConfiguration<Permission>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Code)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(p => p.Description)
            .HasMaxLength(500);
        builder.HasIndex(p => p.Code).IsUnique();

        builder.HasMany(p => p.Roles)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId);
    }
}
