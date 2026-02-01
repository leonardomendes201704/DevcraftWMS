using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Configurations;

public sealed class TransactionLogConfiguration : IEntityTypeConfiguration<TransactionLog>
{
    public void Configure(EntityTypeBuilder<TransactionLog> builder)
    {
        builder.ToTable("TransactionLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntityName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(256);
        builder.Property(x => x.Operation).HasMaxLength(16).IsRequired();
        builder.Property(x => x.OperationId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.CorrelationId).HasMaxLength(128);
        builder.Property(x => x.RequestId).HasMaxLength(128);
        builder.Property(x => x.TraceId).HasMaxLength(128);
        builder.Property(x => x.SpanId).HasMaxLength(128);
        builder.Property(x => x.TenantId).HasMaxLength(128);

        builder.HasIndex(x => x.CorrelationId);
        builder.HasIndex(x => x.RequestId);
        builder.HasIndex(x => x.EntityName);
        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}

