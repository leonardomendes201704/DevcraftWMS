namespace PerfectApiTemplate.Application.Abstractions.Logging;

public interface IRequestLogWriter
{
    void Enqueue(RequestLogEntry entry);
}

public interface IErrorLogWriter
{
    void Enqueue(ErrorLogEntry entry);
}

public interface ITransactionLogWriter
{
    void Enqueue(TransactionLogEntry entry);
}

