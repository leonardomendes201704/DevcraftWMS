using DevcraftWMS.Application.Abstractions;

namespace DevcraftWMS.Infrastructure.Services;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}


