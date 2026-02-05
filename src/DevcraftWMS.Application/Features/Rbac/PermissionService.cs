using Microsoft.Extensions.Logging;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Rbac;

public sealed class PermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(IPermissionRepository permissionRepository, ILogger<PermissionService> logger)
    {
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<RequestResult<PagedResult<PermissionDto>>> ListAsync(
        string? search,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _permissionRepository.CountAsync(search, isActive, includeInactive, cancellationToken);
        var items = await _permissionRepository.ListAsync(pageNumber, pageSize, orderBy, orderDir, search, isActive, includeInactive, cancellationToken);
        var mapped = items.Select(MapPermission).ToList();
        return RequestResult<PagedResult<PermissionDto>>.Success(new PagedResult<PermissionDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir));
    }

    public async Task<RequestResult<PermissionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        if (permission is null)
        {
            return RequestResult<PermissionDto>.Failure("rbac.permission.not_found", "Permission not found.");
        }

        return RequestResult<PermissionDto>.Success(MapPermission(permission));
    }

    public async Task<RequestResult<PermissionDto>> CreateAsync(string code, string name, string? description, CancellationToken cancellationToken)
    {
        if (await _permissionRepository.CodeExistsAsync(code, null, cancellationToken))
        {
            return RequestResult<PermissionDto>.Failure("rbac.permission.code_exists", "Permission code already exists.");
        }

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Code = code.Trim(),
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsActive = true
        };

        await _permissionRepository.AddAsync(permission, cancellationToken);
        _logger.LogInformation("Permission created: {PermissionCode}", permission.Code);

        return RequestResult<PermissionDto>.Success(MapPermission(permission));
    }

    public async Task<RequestResult<PermissionDto>> UpdateAsync(Guid id, string code, string name, string? description, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (permission is null)
        {
            return RequestResult<PermissionDto>.Failure("rbac.permission.not_found", "Permission not found.");
        }

        if (await _permissionRepository.CodeExistsAsync(code, id, cancellationToken))
        {
            return RequestResult<PermissionDto>.Failure("rbac.permission.code_exists", "Permission code already exists.");
        }

        permission.Code = code.Trim();
        permission.Name = name.Trim();
        permission.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        _logger.LogInformation("Permission updated: {PermissionId}", permission.Id);

        return RequestResult<PermissionDto>.Success(MapPermission(permission));
    }

    public async Task<RequestResult<string>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (permission is null)
        {
            return RequestResult<string>.Failure("rbac.permission.not_found", "Permission not found.");
        }

        permission.IsActive = false;
        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        _logger.LogInformation("Permission deactivated: {PermissionId}", permission.Id);

        return RequestResult<string>.Success("Permission deactivated.");
    }

    private static PermissionDto MapPermission(Permission permission)
        => new(permission.Id, permission.Code, permission.Name, permission.Description, permission.IsActive);
}
