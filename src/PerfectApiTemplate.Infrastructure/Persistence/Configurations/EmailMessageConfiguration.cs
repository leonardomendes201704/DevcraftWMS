using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Configurations;

public sealed class EmailMessageConfiguration : AuditableEntityConfiguration<EmailMessage>
{
    protected override void ConfigureEntity(EntityTypeBuilder<EmailMessage> builder)
    {
        builder.ToTable("EmailMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.From).HasMaxLength(320).IsRequired();
        builder.Property(x => x.To).HasMaxLength(320).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(998).IsRequired();
        builder.Property(x => x.Body).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.ProviderMessageId).HasMaxLength(200);
        builder.HasIndex(x => x.Status);
        builder.HasMany(x => x.DeliveryAttempts)
            .WithOne(x => x.EmailMessage)
            .HasForeignKey(x => x.EmailMessageId);
    }
}

