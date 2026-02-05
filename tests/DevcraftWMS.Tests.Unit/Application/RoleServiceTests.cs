using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.Rbac;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class RoleServiceTests
{
    [Fact]
    public async Task CreateRole_Should_Return_Failure_When_Name_Exists()
    {
        var roleRepository = new FakeRoleRepository(nameExists: true);
        var permissionRepository = new FakePermissionRepository();
        var service = new RoleService(roleRepository, permissionRepository, NullLogger<RoleService>.Instance);

        var result = await service.CreateAsync("Admin", null, Array.Empty<Guid>(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("rbac.role.name_exists");
    }

    [Fact]
    public async Task CreateRole_Should_Return_Failure_When_Permission_Not_Found()
    {
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var service = new RoleService(roleRepository, permissionRepository, NullLogger<RoleService>.Instance);

        var result = await service.CreateAsync("Backoffice", null, new[] { Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("rbac.role.permission_not_found");
    }

    private sealed class FakeRoleRepository : IRoleRepository
    {
        private readonly bool _nameExists;
        private readonly Role? _role;

        public FakeRoleRepository(bool nameExists = false, Role? role = null)
        {
            _nameExists = nameExists;
            _role = role;
        }

        public Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default) => Task.FromResult(_nameExists);
        public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_role?.Id == id ? _role : null);
        public Task<Role?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_role?.Id == id ? _role : null);
        public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => Task.FromResult<Role?>(null);
        public Task<IReadOnlyList<Role>> ListByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Role>>(Array.Empty<Role>());
        public Task<int> CountAsync(string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Role>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Role>>(Array.Empty<Role>());
        public Task AddAsync(Role role, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Role role, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakePermissionRepository : IPermissionRepository
    {
        public Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Permission?>(null);
        public Task<Permission?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Permission?>(null);
        public Task<int> CountAsync(string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Permission>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Permission>>(Array.Empty<Permission>());
        public Task<IReadOnlyList<Permission>> ListByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Permission>>(Array.Empty<Permission>());
        public Task AddAsync(Permission permission, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
