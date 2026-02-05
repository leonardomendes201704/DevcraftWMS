using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using DevcraftWMS.Infrastructure.Persistence;

namespace DevcraftWMS.Infrastructure.Auth;

public sealed class RbacUserSeeder
{
    private const string DefaultPassword = "Naotemsenha0!";
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RbacUserSeeder> _logger;

    public RbacUserSeeder(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        ILogger<RbacUserSeeder> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var users = BuildSeedUsers();
        var roleByName = await _dbContext.Roles
            .AsNoTracking()
            .ToDictionaryAsync(r => r.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var seed in users)
        {
            var email = seed.Email.Trim().ToLowerInvariant();
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user is null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    FullName = seed.FullName,
                    PasswordHash = _passwordHasher.Hash(DefaultPassword),
                    IsActive = true,
                    Role = seed.LegacyRole
                };
                _dbContext.Users.Add(user);
            }
            else
            {
                user.IsActive = true;
                user.Role = seed.LegacyRole;
                if (string.IsNullOrWhiteSpace(user.PasswordHash) || !_passwordHasher.Verify(user.PasswordHash, DefaultPassword))
                {
                    user.PasswordHash = _passwordHasher.Hash(DefaultPassword);
                }
            }

            if (!roleByName.TryGetValue(seed.RoleName, out var role))
            {
                _logger.LogWarning("RBAC seed user skipped because role was not found: {RoleName}", seed.RoleName);
                continue;
            }

            var hasAssignment = await _dbContext.UserRoles.AnyAsync(
                ur => ur.UserId == user.Id && ur.RoleId == role.Id,
                cancellationToken);

            if (!hasAssignment)
            {
                _dbContext.UserRoles.Add(new UserRoleAssignment
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("RBAC users seeded: {UserCount}", users.Count);
    }

    private static IReadOnlyList<SeedUser> BuildSeedUsers()
        => new List<SeedUser>
        {
            new("portaria@devcraft.com.br", "Portaria User", UserRole.Portaria, UserRole.Portaria.ToString()),
            new("cliente@devcraft.com.br", "Cliente User", UserRole.Cliente, UserRole.Cliente.ToString()),
            new("supervisor@devcraft.com.br", "Supervisor User", UserRole.Supervisor, UserRole.Supervisor.ToString()),
            new("operador@devcraft.com.br", "Operador User", UserRole.Operador, UserRole.Operador.ToString()),
            new("backoffice@devcraft.com.br", "Backoffice User", UserRole.Backoffice, UserRole.Backoffice.ToString()),
            new("expedicao@devcraft.com.br", "Expedicao User", UserRole.Expedicao, UserRole.Expedicao.ToString()),
            new("picking@devcraft.com.br", "Picking User", UserRole.Picking, UserRole.Picking.ToString())
        };

    private sealed record SeedUser(string Email, string FullName, UserRole LegacyRole, string RoleName);
}
