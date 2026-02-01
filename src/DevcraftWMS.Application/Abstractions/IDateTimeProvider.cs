namespace DevcraftWMS.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}


