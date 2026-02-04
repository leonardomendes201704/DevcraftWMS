namespace DevcraftWMS.Portaria.Infrastructure;

public interface IPortariaUserContext
{
    string? UserId { get; }
    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsInRole(string role);
}
