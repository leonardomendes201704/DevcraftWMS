using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Queues;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Writers;

public sealed class TransactionLogWriter : ITransactionLogWriter
{
    private readonly LogQueue<TransactionLog> _queue;

    public TransactionLogWriter(LogQueue<TransactionLog> queue)
    {
        _queue = queue;
    }

    public void Enqueue(TransactionLogEntry entry)
    {
        var log = new TransactionLog
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = entry.CreatedAtUtc,
            EntityName = entry.EntityName,
            EntityId = entry.EntityId,
            Operation = entry.Operation,
            BeforeJson = entry.BeforeJson,
            AfterJson = entry.AfterJson,
            ChangedProperties = entry.ChangedProperties,
            DurationMs = entry.DurationMs,
            OperationId = entry.OperationId,
            UserId = entry.UserId,
            TenantId = entry.TenantId,
            CorrelationId = entry.CorrelationId,
            RequestId = entry.RequestId,
            TraceId = entry.TraceId,
            SpanId = entry.SpanId
        };

        _queue.Enqueue(log);
    }
}

