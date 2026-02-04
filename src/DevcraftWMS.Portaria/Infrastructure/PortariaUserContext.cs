using System.IdentityModel.Tokens.Jwt;

namespace DevcraftWMS.Portaria.Infrastructure;

public sealed class PortariaUserContext : IPortariaUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Lazy<UserSnapshot> _snapshot;

    public PortariaUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _snapshot = new Lazy<UserSnapshot>(ParseToken);
    }

    public string? UserId => _snapshot.Value.UserId;
    public string? Email => _snapshot.Value.Email;
    public IReadOnlyList<string> Roles => _snapshot.Value.Roles;

    public bool IsInRole(string role)
        => Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));

    private UserSnapshot ParseToken()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetStringValue(SessionKeys.JwtToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            return UserSnapshot.Empty;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var roles = jwt.Claims
                .Where(c => string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(c.Type, "roles", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(c.Type, System.Security.Claims.ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var userId = jwt.Claims.FirstOrDefault(c =>
                string.Equals(c.Type, "sub", StringComparison.OrdinalIgnoreCase)
                || string.Equals(c.Type, "userId", StringComparison.OrdinalIgnoreCase))?.Value;

            var email = jwt.Claims.FirstOrDefault(c =>
                string.Equals(c.Type, "email", StringComparison.OrdinalIgnoreCase)
                || string.Equals(c.Type, System.Security.Claims.ClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value;

            return new UserSnapshot(userId, email, roles);
        }
        catch
        {
            return UserSnapshot.Empty;
        }
    }

    private sealed record UserSnapshot(string? UserId, string? Email, IReadOnlyList<string> Roles)
    {
        public static readonly UserSnapshot Empty = new(null, null, Array.Empty<string>());
    }
}
