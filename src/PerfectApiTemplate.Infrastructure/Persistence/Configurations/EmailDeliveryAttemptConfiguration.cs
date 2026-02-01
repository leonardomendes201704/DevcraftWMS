using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Configurations;

public sealed class EmailDeliveryAttemptConfiguration : AuditableEntityConfiguration<EmailDeliveryAttempt>
{
    protected override void ConfigureEntity(EntityTypeBuilder<EmailDeliveryAttempt> builder)
    {
        builder.ToTable("EmailDeliveryAttempts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmailMessageId).IsRequired();
        builder.Property(x => x.AttemptedAtUtc).IsRequired();
        builder.Property(x => x.Error).HasMaxLength(2000);
        builder.HasIndex(x => x.EmailMessageId);
    }
}

