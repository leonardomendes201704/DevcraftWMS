using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : AuditableEntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email)
            .HasMaxLength(320)
            .IsRequired();
        builder.Property(u => u.FullName)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(u => u.LastLoginUtc);
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
