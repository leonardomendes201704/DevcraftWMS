using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.Rbac;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class PermissionServiceTests
{
    [Fact]
    public async Task CreatePermission_Should_Return_Failure_When_Code_Exists()
    {
        var repository = new FakePermissionRepository(codeExists: true);
        var service = new PermissionService(repository, NullLogger<PermissionService>.Instance);

        var result = await service.CreateAsync("rbac.test", "Test", null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("rbac.permission.code_exists");
    }

    [Fact]
    public async Task UpdatePermission_Should_Return_Failure_When_Not_Found()
    {
        var repository = new FakePermissionRepository();
        var service = new PermissionService(repository, NullLogger<PermissionService>.Instance);

        var result = await service.UpdateAsync(Guid.NewGuid(), "rbac.test", "Test", null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("rbac.permission.not_found");
    }

    private sealed class FakePermissionRepository : IPermissionRepository
    {
        private readonly bool _codeExists;
        private readonly Permission? _permission;

        public FakePermissionRepository(bool codeExists = false, Permission? permission = null)
        {
            _codeExists = codeExists;
            _permission = permission;
        }

        public Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);
        public Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_permission?.Id == id ? _permission : null);
        public Task<Permission?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_permission?.Id == id ? _permission : null);
        public Task<int> CountAsync(string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Permission>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Permission>>(Array.Empty<Permission>());
        public Task<IReadOnlyList<Permission>> ListByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Permission>>(Array.Empty<Permission>());
        public Task AddAsync(Permission permission, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
