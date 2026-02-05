using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IUserRoleAssignmentRepository
{
    Task<IReadOnlyList<Role>> ListRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRoleAssignment>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default);
    Task RemoveAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default);
    Task RemoveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
