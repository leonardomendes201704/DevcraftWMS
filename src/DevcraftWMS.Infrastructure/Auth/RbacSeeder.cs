using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using DevcraftWMS.Infrastructure.Persistence;

namespace DevcraftWMS.Infrastructure.Auth;

public sealed class RbacSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<RbacSeeder> _logger;

    public RbacSeeder(ApplicationDbContext dbContext, ILogger<RbacSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var permissions = SeedPermissions();
        var roles = SeedRoles();

        var existingPermissions = await _dbContext.Permissions
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var permissionByCode = existingPermissions
            .ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);

        foreach (var permission in permissions)
        {
            if (!permissionByCode.TryGetValue(permission.Code, out var existing))
            {
                var entity = new Permission
                {
                    Id = Guid.NewGuid(),
                    Code = permission.Code,
                    Name = permission.Name,
                    Description = permission.Description,
                    IsActive = true
                };

                _dbContext.Permissions.Add(entity);
                permissionByCode[entity.Code] = entity;
                continue;
            }

            if (existing.Name != permission.Name || existing.Description != permission.Description || !existing.IsActive)
            {
                var tracked = await _dbContext.Permissions.FirstAsync(p => p.Id == existing.Id, cancellationToken);
                tracked.Name = permission.Name;
                tracked.Description = permission.Description;
                tracked.IsActive = true;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var existingRoles = await _dbContext.Roles
            .Include(r => r.Permissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync(cancellationToken);

        var roleByName = existingRoles
            .ToDictionary(r => r.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var roleSeed in roles)
        {
            if (!roleByName.TryGetValue(roleSeed.Name, out var role))
            {
                role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleSeed.Name,
                    Description = roleSeed.Description,
                    IsActive = true
                };
                _dbContext.Roles.Add(role);
                roleByName[role.Name] = role;
            }
            else
            {
                role.Description = roleSeed.Description;
                role.IsActive = true;
            }

            var existingPermissionIds = role.Permissions.Select(rp => rp.PermissionId).ToHashSet();
            foreach (var permissionCode in roleSeed.PermissionCodes)
            {
                if (!permissionByCode.TryGetValue(permissionCode, out var permission))
                {
                    continue;
                }

                if (existingPermissionIds.Contains(permission.Id))
                {
                    continue;
                }

                role.Permissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = role.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await EnsureUserRoleAssignmentsAsync(roleByName, cancellationToken);
        _logger.LogInformation("RBAC seed completed with {RoleCount} roles and {PermissionCount} permissions.", roleByName.Count, permissionByCode.Count);
    }

    private async Task EnsureUserRoleAssignmentsAsync(Dictionary<string, Role> roleByName, CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
        foreach (var user in users)
        {
            var legacyRoleName = user.Role.ToString();
            if (!roleByName.TryGetValue(legacyRoleName, out var role))
            {
                continue;
            }

            var hasAssignment = await _dbContext.UserRoles.AnyAsync(
                ur => ur.UserId == user.Id && ur.RoleId == role.Id,
                cancellationToken);

            if (hasAssignment)
            {
                continue;
            }

            _dbContext.UserRoles.Add(new UserRoleAssignment
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = role.Id
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyList<SeedPermission> SeedPermissions()
        => new List<SeedPermission>
        {
            new("rbac.roles.manage", "Manage roles", "Create and update access roles."),
            new("rbac.permissions.manage", "Manage permissions", "Create and update permissions."),
            new("rbac.users.manage", "Manage users", "Create and update internal users."),
            new("customers.manage", "Manage customers", "Maintain customer registry."),
            new("warehouses.manage", "Manage warehouses", "Maintain warehouse registry."),
            new("inventory.manage", "Manage inventory", "Manage inventory balances and movements."),
            new("receipts.manage", "Manage receipts", "Receive inbound goods and register receipts."),
            new("quality.manage", "Manage quality", "Execute quality inspections."),
            new("putaway.manage", "Manage putaway", "Assign and complete putaway tasks."),
            new("gatecheckin.manage", "Manage gate check-ins", "Register gate check-ins."),
            new("picking.manage", "Manage picking", "Execute picking tasks."),
            new("expedition.manage", "Manage expedition", "Execute expedition tasks."),
            new("portal.view", "View portal", "Access portal data."),
            new("reports.view", "View reports", "Access operational reports."),
            new("logs.view", "View logs", "Access request and error logs.")
        };

    private static IReadOnlyList<SeedRole> SeedRoles()
        => new List<SeedRole>
        {
            new(
                UserRole.Admin.ToString(),
                "Full system administrator",
                new[]
                {
                    "rbac.roles.manage",
                    "rbac.permissions.manage",
                    "rbac.users.manage",
                    "customers.manage",
                    "warehouses.manage",
                    "inventory.manage",
                    "receipts.manage",
                    "quality.manage",
                    "putaway.manage",
                    "gatecheckin.manage",
                    "reports.view",
                    "logs.view"
                }),
            new(
                UserRole.Backoffice.ToString(),
                "Backoffice operations",
                new[]
                {
                    "customers.manage",
                    "warehouses.manage",
                    "inventory.manage",
                    "receipts.manage",
                    "quality.manage",
                    "putaway.manage",
                    "reports.view",
                    "logs.view"
                }),
            new(UserRole.Portaria.ToString(), "Gate control", new[] { "gatecheckin.manage" }),
            new(UserRole.Conferente.ToString(), "Receiving operator", new[] { "receipts.manage", "inventory.manage" }),
            new(UserRole.Qualidade.ToString(), "Quality control", new[] { "quality.manage" }),
            new(UserRole.Putaway.ToString(), "Putaway operator", new[] { "putaway.manage" }),
            new(
                UserRole.Supervisor.ToString(),
                "Supervisor operations",
                new[]
                {
                    "customers.manage",
                    "warehouses.manage",
                    "inventory.manage",
                    "receipts.manage",
                    "quality.manage",
                    "putaway.manage",
                    "reports.view",
                    "logs.view"
                }),
            new(UserRole.Operador.ToString(), "Operator", new[] { "receipts.manage", "inventory.manage" }),
            new(UserRole.Cliente.ToString(), "Customer portal", new[] { "portal.view", "reports.view" }),
            new(UserRole.Expedicao.ToString(), "Expedition", new[] { "expedition.manage", "inventory.manage", "reports.view" }),
            new(UserRole.Picking.ToString(), "Picking", new[] { "picking.manage", "inventory.manage" })
        };

    private sealed record SeedPermission(string Code, string Name, string? Description);
    private sealed record SeedRole(string Name, string? Description, IReadOnlyList<string> PermissionCodes);
}
