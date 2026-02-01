using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Application.Abstractions;

public interface ICustomerRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? search,
        string? email,
        string? name,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? search, string? email, string? name, bool includeInactive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> ListByCreatedAtCursorAsync(int pageSize, string orderDir, DateTime cursorTime, Guid cursorId, bool includeInactive, CancellationToken cancellationToken = default);
}
