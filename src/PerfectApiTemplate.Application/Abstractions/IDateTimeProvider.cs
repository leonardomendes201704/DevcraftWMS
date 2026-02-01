namespace PerfectApiTemplate.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

