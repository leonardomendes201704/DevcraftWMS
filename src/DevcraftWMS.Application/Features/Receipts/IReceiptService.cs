using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Receipts;

public interface IReceiptService
{
    Task<RequestResult<ReceiptDetailDto>> CreateReceiptAsync(
        Guid warehouseId,
        string receiptNumber,
        string? documentNumber,
        string? supplierName,
        string? notes,
        CancellationToken cancellationToken);
    Task<RequestResult<ReceiptDetailDto>> UpdateReceiptAsync(
        Guid id,
        Guid warehouseId,
        string receiptNumber,
        string? documentNumber,
        string? supplierName,
        string? notes,
        CancellationToken cancellationToken);
    Task<RequestResult<ReceiptDetailDto>> DeactivateReceiptAsync(Guid id, CancellationToken cancellationToken);
    Task<RequestResult<ReceiptItemDto>> AddItemAsync(
        Guid receiptId,
        Guid productId,
        Guid? lotId,
        string? lotCode,
        DateOnly? expirationDate,
        Guid locationId,
        Guid uomId,
        decimal quantity,
        decimal? unitCost,
        decimal? actualWeightKg,
        decimal? actualVolumeCm3,
        CancellationToken cancellationToken);
    Task<RequestResult<ReceiptDetailDto>> StartFromInboundOrderAsync(Guid inboundOrderId, CancellationToken cancellationToken);
    Task<RequestResult<ReceiptDetailDto>> CompleteAsync(Guid receiptId, CancellationToken cancellationToken);
}
