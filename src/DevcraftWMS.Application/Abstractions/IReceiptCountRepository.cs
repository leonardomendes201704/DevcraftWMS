using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IReceiptCountRepository
{
    Task AddAsync(ReceiptCount receiptCount, CancellationToken cancellationToken = default);
    Task UpdateAsync(ReceiptCount receiptCount, CancellationToken cancellationToken = default);
    Task<ReceiptCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReceiptCount?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReceiptCount?> GetByReceiptItemAsync(Guid receiptId, Guid inboundOrderItemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReceiptCount>> ListByReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default);
}
