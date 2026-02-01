using Microsoft.EntityFrameworkCore;
using PerfectApiTemplate.Application.Abstractions;
using PerfectApiTemplate.Domain.Entities;
using PerfectApiTemplate.Infrastructure.Persistence;

namespace PerfectApiTemplate.Infrastructure.Persistence;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CustomerRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .AnyAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .AnyAsync(c => c.Email == email && c.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _dbContext.Customers.Update(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? search,
        string? email,
        string? name,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Customers.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search) || c.Email.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(c => c.Email.Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }

        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(string? search, string? email, string? name, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Customers.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search) || c.Email.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(c => c.Email.Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> ListByCreatedAtCursorAsync(int pageSize, string orderDir, DateTime cursorTime, Guid cursorId, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Customers.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        if (string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase))
        {
            query = query
                .Where(c => c.CreatedAtUtc > cursorTime || (c.CreatedAtUtc == cursorTime && c.Id.CompareTo(cursorId) > 0))
                .OrderBy(c => c.CreatedAtUtc)
                .ThenBy(c => c.Id);
        }
        else
        {
            query = query
                .Where(c => c.CreatedAtUtc < cursorTime || (c.CreatedAtUtc == cursorTime && c.Id.CompareTo(cursorId) < 0))
                .OrderByDescending(c => c.CreatedAtUtc)
                .ThenByDescending(c => c.Id);
        }

        return await query.Take(pageSize).ToListAsync(cancellationToken);
    }

    private static IQueryable<Customer> ApplyOrdering(IQueryable<Customer> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "name" => asc ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
            "email" => asc ? query.OrderBy(c => c.Email) : query.OrderByDescending(c => c.Email),
            "dateofbirth" => asc ? query.OrderBy(c => c.DateOfBirth) : query.OrderByDescending(c => c.DateOfBirth),
            "createdatutc" => asc ? query.OrderBy(c => c.CreatedAtUtc) : query.OrderByDescending(c => c.CreatedAtUtc),
            _ => asc ? query.OrderBy(c => c.CreatedAtUtc) : query.OrderByDescending(c => c.CreatedAtUtc)
        };
    }
}
