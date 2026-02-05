using Microsoft.Extensions.Logging;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Rbac;

public sealed class RoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<RequestResult<PagedResult<RoleListItemDto>>> ListAsync(
        string? search,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _roleRepository.CountAsync(search, isActive, includeInactive, cancellationToken);
        var items = await _roleRepository.ListAsync(pageNumber, pageSize, orderBy, orderDir, search, isActive, includeInactive, cancellationToken);
        var mapped = items.Select(r => new RoleListItemDto(r.Id, r.Name, r.IsActive, r.CreatedAtUtc)).ToList();
        return RequestResult<PagedResult<RoleListItemDto>>.Success(new PagedResult<RoleListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir));
    }

    public async Task<RequestResult<RoleDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return RequestResult<RoleDetailDto>.Failure("rbac.role.not_found", "Role not found.");
        }

        return RequestResult<RoleDetailDto>.Success(MapRole(role));
    }

    public async Task<RequestResult<RoleDetailDto>> CreateAsync(string name, string? description, IReadOnlyList<Guid> permissionIds, CancellationToken cancellationToken)
    {
        if (await _roleRepository.NameExistsAsync(name, null, cancellationToken))
        {
            return RequestResult<RoleDetailDto>.Failure("rbac.role.name_exists", "Role name already exists.");
        }

        var permissions = await _permissionRepository.ListByIdsAsync(permissionIds, cancellationToken);
        if (permissions.Count != permissionIds.Count)
        {
            return RequestResult<RoleDetailDto>.Failure("rbac.role.permission_not_found", "One or more permissions were not found.");
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsActive = true
        };

        foreach (var permission in permissions)
        {
            role.Permissions.Add(new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                PermissionId = permission.Id,
                Permission = permission
            });
        }

        await _roleRepository.AddAsync(role, cancellationToken);
        _logger.LogInformation("Role created: {RoleName}", role.Name);

        return RequestResult<RoleDetailDto>.Success(MapRole(role));
    }

    public async Task<RequestResult<RoleDetailDto>> UpdateAsync(Guid id, string name, string? description, IReadOnlyList<Guid> permissionIds, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return RequestResult<RoleDetailDto>.Failure("rbac.role.not_found", "Role not found.");
        }

        if (await _roleRepository.NameExistsAsync(name, id, cancellationToken))
        {
            return RequestResult<RoleDetailDto>.Failure("rbac.role.name_exists", "Role name already exists.");
        }

        var permissions = await _permissionRepository.ListByIdsAsync(permissionIds, cancellationToken);
        if (permissions.Count != permissionIds.Count)
        {
            return RequestResult<RoleDetailDto>.Failure("rbac.role.permission_not_found", "One or more permissions were not found.");
        }

        role.Name = name.Trim();
        role.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        var desiredPermissionIds = permissions.Select(p => p.Id).ToHashSet();
        var existingPermissionIds = role.Permissions.Select(rp => rp.PermissionId).ToHashSet();

        var toRemove = role.Permissions.Where(rp => !desiredPermissionIds.Contains(rp.PermissionId)).ToList();
        foreach (var rolePermission in toRemove)
        {
            role.Permissions.Remove(rolePermission);
        }

        foreach (var permission in permissions.Where(p => !existingPermissionIds.Contains(p.Id)))
        {
            role.Permissions.Add(new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                PermissionId = permission.Id,
                Permission = permission
            });
        }

        await _roleRepository.UpdateAsync(role, cancellationToken);
        _logger.LogInformation("Role updated: {RoleId}", role.Id);

        return RequestResult<RoleDetailDto>.Success(MapRole(role));
    }

    public async Task<RequestResult<string>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return RequestResult<string>.Failure("rbac.role.not_found", "Role not found.");
        }

        role.IsActive = false;
        await _roleRepository.UpdateAsync(role, cancellationToken);
        _logger.LogInformation("Role deactivated: {RoleId}", role.Id);

        return RequestResult<string>.Success("Role deactivated.");
    }

    private static RoleDetailDto MapRole(Role role)
    {
        var permissions = role.Permissions
            .Select(rp => rp.Permission)
            .Where(p => p is not null)
            .Select(p => new PermissionDto(p!.Id, p.Code, p.Name, p.Description, p.IsActive))
            .ToList();

        return new RoleDetailDto(role.Id, role.Name, role.Description, role.IsActive, permissions);
    }
}
