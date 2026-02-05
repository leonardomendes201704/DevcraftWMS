using Microsoft.Extensions.Logging;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Users;

public sealed class UserManagementService
{
    private const string ResetPassword = "Mudar@123";
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleAssignmentRepository _userRoleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        IUserRepository userRepository,
        IUserRoleAssignmentRepository userRoleRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        ILogger<UserManagementService> logger)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<RequestResult<PagedResult<UserListItemDto>>> ListAsync(
        string? search,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _userRepository.CountAsync(search, isActive, includeInactive, cancellationToken);
        var items = await _userRepository.ListAsync(pageNumber, pageSize, orderBy, orderDir, search, isActive, includeInactive, cancellationToken);
        var mapped = items.Select(u => new UserListItemDto(u.Id, u.Email, u.FullName, u.IsActive, u.CreatedAtUtc)).ToList();
        return RequestResult<PagedResult<UserListItemDto>>.Success(new PagedResult<UserListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir));
    }

    public async Task<RequestResult<UserDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return RequestResult<UserDetailDto>.Failure("rbac.user.not_found", "User not found.");
        }

        var roles = await _userRoleRepository.ListRolesByUserIdAsync(user.Id, cancellationToken);
        return RequestResult<UserDetailDto>.Success(new UserDetailDto(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc, roles.Select(r => r.Name).ToList()));
    }

    public async Task<RequestResult<UserDetailDto>> CreateAsync(string email, string fullName, string password, IReadOnlyList<Guid> roleIds, CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(email, null, cancellationToken))
        {
            return RequestResult<UserDetailDto>.Failure("rbac.user.email_exists", "Email already exists.");
        }

        var roles = await LoadRolesAsync(roleIds, cancellationToken);
        if (roles.Count != roleIds.Count)
        {
            return RequestResult<UserDetailDto>.Failure("rbac.user.role_not_found", "One or more roles were not found.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            FullName = fullName.Trim(),
            PasswordHash = _passwordHasher.Hash(password),
            IsActive = true,
            Role = ResolveLegacyRole(roles)
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await ReplaceRolesAsync(user.Id, roles, cancellationToken);
        _logger.LogInformation("User created: {Email}", user.Email);

        return RequestResult<UserDetailDto>.Success(new UserDetailDto(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc, roles.Select(r => r.Name).ToList()));
    }

    public async Task<RequestResult<UserDetailDto>> UpdateAsync(Guid id, string email, string fullName, IReadOnlyList<Guid> roleIds, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return RequestResult<UserDetailDto>.Failure("rbac.user.not_found", "User not found.");
        }

        if (await _userRepository.EmailExistsAsync(email, id, cancellationToken))
        {
            return RequestResult<UserDetailDto>.Failure("rbac.user.email_exists", "Email already exists.");
        }

        var roles = await LoadRolesAsync(roleIds, cancellationToken);
        if (roles.Count != roleIds.Count)
        {
            return RequestResult<UserDetailDto>.Failure("rbac.user.role_not_found", "One or more roles were not found.");
        }

        user.Email = email.Trim().ToLowerInvariant();
        user.FullName = fullName.Trim();
        user.Role = ResolveLegacyRole(roles);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await ReplaceRolesAsync(user.Id, roles, cancellationToken);
        _logger.LogInformation("User updated: {UserId}", user.Id);

        return RequestResult<UserDetailDto>.Success(new UserDetailDto(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc, roles.Select(r => r.Name).ToList()));
    }

    public async Task<RequestResult<string>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return RequestResult<string>.Failure("rbac.user.not_found", "User not found.");
        }

        user.IsActive = false;
        await _userRepository.UpdateAsync(user, cancellationToken);
        _logger.LogInformation("User deactivated: {UserId}", user.Id);
        return RequestResult<string>.Success("User deactivated.");
    }

    public async Task<RequestResult<string>> ResetPasswordAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return RequestResult<string>.Failure("rbac.user.not_found", "User not found.");
        }

        user.PasswordHash = _passwordHasher.Hash(ResetPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        _logger.LogInformation("User password reset: {UserId}", user.Id);
        return RequestResult<string>.Success("Password reset to default.");
    }

    public async Task<RequestResult<string>> AssignRolesAsync(Guid id, IReadOnlyList<Guid> roleIds, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return RequestResult<string>.Failure("rbac.user.not_found", "User not found.");
        }

        var roles = await LoadRolesAsync(roleIds, cancellationToken);
        if (roles.Count != roleIds.Count)
        {
            return RequestResult<string>.Failure("rbac.user.role_not_found", "One or more roles were not found.");
        }

        await ReplaceRolesAsync(user.Id, roles, cancellationToken);
        user.Role = ResolveLegacyRole(roles);
        await _userRepository.UpdateAsync(user, cancellationToken);
        _logger.LogInformation("User roles updated: {UserId}", user.Id);
        return RequestResult<string>.Success("User roles updated.");
    }

    private async Task<IReadOnlyList<Role>> LoadRolesAsync(IReadOnlyList<Guid> roleIds, CancellationToken cancellationToken)
        => await _roleRepository.ListByIdsAsync(roleIds, cancellationToken);

    private async Task ReplaceRolesAsync(Guid userId, IReadOnlyList<Role> roles, CancellationToken cancellationToken)
    {
        await _userRoleRepository.RemoveByUserIdAsync(userId, cancellationToken);
        foreach (var role in roles)
        {
            var assignment = new UserRoleAssignment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = role.Id
            };
            await _userRoleRepository.AddAsync(assignment, cancellationToken);
        }
    }

    private static UserRole ResolveLegacyRole(IReadOnlyList<Role> roles)
    {
        if (roles.Count == 0)
        {
            return UserRole.Backoffice;
        }

        foreach (var role in roles)
        {
            if (Enum.TryParse<UserRole>(role.Name, out var parsed))
            {
                return parsed;
            }
        }

        return UserRole.Backoffice;
    }
}
