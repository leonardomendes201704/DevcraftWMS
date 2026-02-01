using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Configurations;

public abstract class AuditableEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : AuditableEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        ConfigureEntity(builder);
        ConfigureAudit(builder);
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);

    protected virtual void ConfigureAudit(EntityTypeBuilder<T> builder)
    {
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);
        builder.Property(x => x.DeletedAtUtc);
        builder.Property(x => x.CreatedByUserId);
        builder.Property(x => x.UpdatedByUserId);
        builder.Property(x => x.DeletedByUserId);
        builder.Property(x => x.IsActive).IsRequired();
    }
}
