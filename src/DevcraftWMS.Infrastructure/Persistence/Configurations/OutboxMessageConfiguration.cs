using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : AuditableEntityConfiguration<OutboxMessage>
{
    protected override void ConfigureEntity(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Type)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(o => o.Payload)
            .IsRequired();
        builder.Property(o => o.OccurredAtUtc)
            .IsRequired();
        builder.Property(o => o.ProcessedAtUtc);
        builder.Property(o => o.Error)
            .HasMaxLength(2000);
        builder.HasIndex(o => o.ProcessedAtUtc);
    }
}

