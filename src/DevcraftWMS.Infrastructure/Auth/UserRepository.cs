using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Infrastructure.Persistence;

namespace DevcraftWMS.Infrastructure.Auth;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users.AsNoTracking().Where(u => u.Email == email);
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<int> CountAsync(string? search, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(search, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? search,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(search, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<User> BuildQuery(string? search, bool? isActive, bool includeInactive)
    {
        var query = _dbContext.Users.AsNoTracking();

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(u => u.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(u => u.Email.Contains(term) || u.FullName.Contains(term));
        }

        return query;
    }

    private static IQueryable<User> ApplyOrdering(IQueryable<User> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "email" => asc ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            "fullname" => asc ? query.OrderBy(u => u.FullName) : query.OrderByDescending(u => u.FullName),
            "createdatutc" => asc ? query.OrderBy(u => u.CreatedAtUtc) : query.OrderByDescending(u => u.CreatedAtUtc),
            _ => asc ? query.OrderBy(u => u.CreatedAtUtc) : query.OrderByDescending(u => u.CreatedAtUtc)
        };
    }
}

