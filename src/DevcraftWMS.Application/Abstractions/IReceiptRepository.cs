using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IReceiptRepository
{
    Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default);
    Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default);
    Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Receipt?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        string? receiptNumber,
        string? documentNumber,
        string? supplierName,
        ReceiptStatus? status,
        DateTime? receivedFromUtc,
        DateTime? receivedToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Receipt>> ListAsync(
        Guid? warehouseId,
        string? receiptNumber,
        string? documentNumber,
        string? supplierName,
        ReceiptStatus? status,
        DateTime? receivedFromUtc,
        DateTime? receivedToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
    Task AddItemAsync(ReceiptItem item, CancellationToken cancellationToken = default);
    Task<int> CountItemsAsync(
        Guid receiptId,
        Guid? productId,
        Guid? locationId,
        Guid? lotId,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReceiptItem>> ListItemsAsync(
        Guid receiptId,
        Guid? productId,
        Guid? locationId,
        Guid? lotId,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
}
