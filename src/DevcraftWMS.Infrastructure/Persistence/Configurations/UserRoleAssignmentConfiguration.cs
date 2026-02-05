using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class UserRoleAssignmentConfiguration : AuditableEntityConfiguration<UserRoleAssignment>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.UserId).IsRequired();
        builder.Property(ur => ur.RoleId).IsRequired();
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
    }
}
