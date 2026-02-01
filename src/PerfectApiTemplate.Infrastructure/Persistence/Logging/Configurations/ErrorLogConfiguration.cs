using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Configurations;

public sealed class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.ToTable("ErrorLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ExceptionType).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Method).HasMaxLength(16).IsRequired();
        builder.Property(x => x.Path).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.QueryString).HasMaxLength(2048);
        builder.Property(x => x.RequestId).HasMaxLength(128);
        builder.Property(x => x.ApiRequestId).HasMaxLength(128);
        builder.Property(x => x.CorrelationId).HasMaxLength(128);
        builder.Property(x => x.TraceId).HasMaxLength(128);
        builder.Property(x => x.SpanId).HasMaxLength(128);
        builder.Property(x => x.TenantId).HasMaxLength(128);
        builder.Property(x => x.UserIdText).HasMaxLength(128);
        builder.Property(x => x.EnvironmentName).HasMaxLength(128);
        builder.Property(x => x.MachineName).HasMaxLength(128);
        builder.Property(x => x.AssemblyVersion).HasMaxLength(64);
        builder.Property(x => x.Source).HasMaxLength(32).IsRequired();
        builder.Property(x => x.EventType).HasMaxLength(128);
        builder.Property(x => x.Severity).HasMaxLength(32);
        builder.Property(x => x.ClientApp).HasMaxLength(128);
        builder.Property(x => x.ClientEnv).HasMaxLength(64);
        builder.Property(x => x.ClientUrl).HasMaxLength(2048);
        builder.Property(x => x.ClientRoute).HasMaxLength(256);
        builder.Property(x => x.ApiMethod).HasMaxLength(16);
        builder.Property(x => x.ApiPath).HasMaxLength(1024);
        builder.Property(x => x.UserAgent).HasMaxLength(512);
        builder.Property(x => x.ClientIp).HasMaxLength(64);
        builder.Property(x => x.Tags).HasMaxLength(512);

        builder.HasIndex(x => x.CorrelationId);
        builder.HasIndex(x => x.RequestId);
        builder.HasIndex(x => x.EventType);
        builder.HasIndex(x => x.ExceptionType);
        builder.HasIndex(x => x.CreatedAtUtc);
        builder.HasIndex(x => x.ApiStatusCode);
    }
}
