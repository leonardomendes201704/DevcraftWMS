using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.ReceiptCounts;

public interface IReceiptCountService
{
    Task<RequestResult<IReadOnlyList<ReceiptExpectedItemDto>>> ListExpectedItemsAsync(
        Guid receiptId,
        CancellationToken cancellationToken);
    Task<RequestResult<IReadOnlyList<ReceiptCountDto>>> ListCountsAsync(
        Guid receiptId,
        CancellationToken cancellationToken);
    Task<RequestResult<ReceiptCountDto>> RegisterCountAsync(
        Guid receiptId,
        Guid inboundOrderItemId,
        decimal countedQuantity,
        ReceiptCountMode mode,
        string? notes,
        CancellationToken cancellationToken);
}
