using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Logging;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging;

public sealed class TransactionAuditInterceptor : SaveChangesInterceptor
{
    private readonly ITransactionLogWriter _writer;
    private readonly ICorrelationContext _correlationContext;
    private readonly LoggingOptions _options;
    private readonly ConcurrentDictionary<DbContext, AuditContext> _contexts = new();

    public TransactionAuditInterceptor(
        ITransactionLogWriter writer,
        ICorrelationContext correlationContext,
        IOptions<LoggingOptions> options)
    {
        _writer = writer;
        _correlationContext = correlationContext;
        _options = options.Value;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        CaptureEntries(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        CaptureEntries(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        WriteEntries(eventData.Context);
        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        WriteEntries(eventData.Context);
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _contexts.TryRemove(eventData.Context!, out _);
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        _contexts.TryRemove(eventData.Context!, out _);
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private void CaptureEntries(DbContext? context)
    {
        if (!_options.Transactions.Enabled || context is null)
        {
            return;
        }

        var auditContext = new AuditContext(Guid.NewGuid().ToString("N"), DateTime.UtcNow);
        _contexts[context] = auditContext;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is null || entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
            {
                continue;
            }

            if (entry.Metadata.IsOwned())
            {
                continue;
            }

            var entityName = entry.Entity.GetType().Name;
            if (_options.Transactions.ExcludedEntities.Any(x => string.Equals(x, entityName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var key = string.Join("|", entry.Properties.Where(x => x.Metadata.IsPrimaryKey())
                .Select(x => $"{x.Metadata.Name}:{x.CurrentValue ?? x.OriginalValue}"));

            var before = entry.State == EntityState.Added ? null : ToDictionary(entry.OriginalValues, _options.Transactions.ExcludedProperties);
            var after = entry.State == EntityState.Deleted ? null : ToDictionary(entry.CurrentValues, _options.Transactions.ExcludedProperties);

            var changedProps = entry.State == EntityState.Modified
                ? string.Join(',', entry.Properties.Where(x => x.IsModified).Select(x => x.Metadata.Name))
                : null;

            var beforeJson = before is null ? null : LogMasking.SanitizeJson(JsonSerializer.Serialize(before), _options.Mask.JsonKeys, _options.Mask.JsonPaths, _options.Mask.Enabled);
            var afterJson = after is null ? null : LogMasking.SanitizeJson(JsonSerializer.Serialize(after), _options.Mask.JsonKeys, _options.Mask.JsonPaths, _options.Mask.Enabled);

            auditContext.Entries.Add(new TransactionLogEntry(
                DateTime.UtcNow,
                entityName,
                string.IsNullOrWhiteSpace(key) ? null : key,
                entry.State switch
                {
                    EntityState.Added => "Insert",
                    EntityState.Deleted => "Delete",
                    _ => "Update"
                },
                beforeJson,
                afterJson,
                changedProps,
                0,
                auditContext.OperationId,
                _correlationContext.UserId,
                _correlationContext.TenantId,
                _correlationContext.CorrelationId,
                _correlationContext.RequestId,
                _correlationContext.TraceId,
                _correlationContext.SpanId));
        }
    }

    private void WriteEntries(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        if (!_contexts.TryRemove(context, out var auditContext))
        {
            return;
        }

        var durationMs = (long)(DateTime.UtcNow - auditContext.StartedAtUtc).TotalMilliseconds;
        foreach (var entry in auditContext.Entries)
        {
            _writer.Enqueue(entry with { DurationMs = durationMs });
        }
    }

    private static Dictionary<string, object?> ToDictionary(PropertyValues values, string[] excludedProperties)
    {
        var excluded = new HashSet<string>(excludedProperties, StringComparer.OrdinalIgnoreCase);
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var prop in values.Properties)
        {
            if (excluded.Contains(prop.Name))
            {
                continue;
            }

            dict[prop.Name] = values[prop];
        }

        return dict;
    }


    private sealed class AuditContext
    {
        public AuditContext(string operationId, DateTime startedAtUtc)
        {
            OperationId = operationId;
            StartedAtUtc = startedAtUtc;
        }

        public string OperationId { get; }
        public DateTime StartedAtUtc { get; }
        public List<TransactionLogEntry> Entries { get; } = new();
    }
}
