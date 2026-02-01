using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Configurations;

public sealed class UserProviderConfiguration : AuditableEntityConfiguration<UserProvider>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserProvider> builder)
    {
        builder.ToTable("UserProviders");
        builder.HasKey(up => up.Id);
        builder.Property(up => up.Provider)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(up => up.ProviderUserId)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(up => up.LinkedAtUtc)
            .IsRequired();

        builder.HasOne(up => up.User)
            .WithMany()
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(up => new { up.Provider, up.ProviderUserId }).IsUnique();
    }
}
