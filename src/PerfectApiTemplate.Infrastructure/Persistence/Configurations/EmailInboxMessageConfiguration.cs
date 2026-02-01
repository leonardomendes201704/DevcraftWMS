using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Configurations;

public sealed class EmailInboxMessageConfiguration : AuditableEntityConfiguration<EmailInboxMessage>
{
    protected override void ConfigureEntity(EntityTypeBuilder<EmailInboxMessage> builder)
    {
        builder.ToTable("EmailInboxMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProviderMessageId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.From).HasMaxLength(320).IsRequired();
        builder.Property(x => x.To).HasMaxLength(320).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(998).IsRequired();
        builder.Property(x => x.BodyPreview).HasMaxLength(2000);
        builder.Property(x => x.Status).IsRequired();
        builder.HasIndex(x => x.ProviderMessageId).IsUnique();
        builder.HasIndex(x => x.Status);
    }
}

