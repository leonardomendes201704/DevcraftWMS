using PerfectApiTemplate.Application.Abstractions;

namespace PerfectApiTemplate.Infrastructure.Services;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

