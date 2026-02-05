using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class UserRoleAssignmentRepository : IUserRoleAssignmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRoleAssignmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Role>> ListRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role!)
            .Where(r => r != null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserRoleAssignment>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default)
    {
        _dbContext.UserRoles.Add(assignment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default)
    {
        _dbContext.UserRoles.Remove(assignment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.UserRoles.Where(ur => ur.UserId == userId).ToListAsync(cancellationToken);
        if (items.Count == 0)
        {
            return;
        }

        _dbContext.UserRoles.RemoveRange(items);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
