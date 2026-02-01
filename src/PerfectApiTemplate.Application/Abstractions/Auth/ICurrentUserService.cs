namespace PerfectApiTemplate.Application.Abstractions.Auth;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
}
