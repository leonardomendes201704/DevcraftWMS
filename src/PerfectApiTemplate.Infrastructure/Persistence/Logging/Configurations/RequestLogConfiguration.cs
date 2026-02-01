using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Configurations;

public sealed class RequestLogConfiguration : IEntityTypeConfiguration<RequestLog>
{
    public void Configure(EntityTypeBuilder<RequestLog> builder)
    {
        builder.ToTable("RequestLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).HasMaxLength(16).IsRequired();
        builder.Property(x => x.Scheme).HasMaxLength(16).IsRequired();
        builder.Property(x => x.Host).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Path).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.QueryString).HasMaxLength(2048);
        builder.Property(x => x.RequestContentType).HasMaxLength(256);
        builder.Property(x => x.ResponseContentType).HasMaxLength(256);
        builder.Property(x => x.ClientIp).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(1024);
        builder.Property(x => x.CorrelationId).HasMaxLength(128);
        builder.Property(x => x.RequestId).HasMaxLength(128);
        builder.Property(x => x.TraceId).HasMaxLength(128);
        builder.Property(x => x.SpanId).HasMaxLength(128);
        builder.Property(x => x.TenantId).HasMaxLength(128);

        builder.HasIndex(x => x.CorrelationId);
        builder.HasIndex(x => x.RequestId);
        builder.HasIndex(x => x.Path);
        builder.HasIndex(x => x.StatusCode);
        builder.HasIndex(x => x.StartedAtUtc);
    }
}

