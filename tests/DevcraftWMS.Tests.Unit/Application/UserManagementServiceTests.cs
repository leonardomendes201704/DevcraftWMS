using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Features.Auth;
using DevcraftWMS.Application.Features.Users;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class UserManagementServiceTests
{
    [Fact]
    public async Task CreateUser_Should_Return_Failure_When_Email_Exists()
    {
        var userRepository = new FakeUserRepository(emailExists: true);
        var roleRepository = new FakeRoleRepository();
        var userRoleRepository = new FakeUserRoleRepository();
        var service = new UserManagementService(userRepository, userRoleRepository, roleRepository, new PasswordHasher(), NullLogger<UserManagementService>.Instance);

        var result = await service.CreateAsync("test@example.com", "Test", "ChangeMe123!", Array.Empty<Guid>(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("rbac.user.email_exists");
    }

    [Fact]
    public async Task AssignRoles_Should_Return_Failure_When_Role_Not_Found()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", FullName = "Test" };
        var userRepository = new FakeUserRepository(user: user);
        var roleRepository = new FakeRoleRepository();
        var userRoleRepository = new FakeUserRoleRepository();
        var service = new UserManagementService(userRepository, userRoleRepository, roleRepository, new PasswordHasher(), NullLogger<UserManagementService>.Instance);

        var result = await service.AssignRolesAsync(user.Id, new[] { Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("rbac.user.role_not_found");
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly bool _emailExists;
        private readonly User? _user;

        public FakeUserRepository(bool emailExists = false, User? user = null)
        {
            _emailExists = emailExists;
            _user = user;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => Task.FromResult(_user?.Email == email ? _user : null);
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_user?.Id == id ? _user : null);
        public Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default) => Task.FromResult(_emailExists);
        public Task<int> CountAsync(string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<User>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(Array.Empty<User>());
        public Task AddAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeRoleRepository : IRoleRepository
    {
        public Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Role?>(null);
        public Task<Role?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Role?>(null);
        public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => Task.FromResult<Role?>(null);
        public Task<IReadOnlyList<Role>> ListByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Role>>(Array.Empty<Role>());
        public Task<int> CountAsync(string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Role>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Role>>(Array.Empty<Role>());
        public Task AddAsync(Role role, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Role role, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeUserRoleRepository : IUserRoleAssignmentRepository
    {
        public Task<IReadOnlyList<Role>> ListRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Role>>(Array.Empty<Role>());
        public Task<IReadOnlyList<UserRoleAssignment>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<UserRoleAssignment>>(Array.Empty<UserRoleAssignment>());
        public Task AddAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RemoveAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RemoveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
