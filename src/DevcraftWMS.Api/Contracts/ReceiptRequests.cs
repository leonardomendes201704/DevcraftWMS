namespace DevcraftWMS.Api.Contracts;

public sealed record CreateReceiptRequest(
    Guid WarehouseId,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    string? Notes);

public sealed record UpdateReceiptRequest(
    Guid WarehouseId,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    string? Notes);

public sealed record AddReceiptItemRequest(
    Guid ProductId,
    Guid? LotId,
    Guid LocationId,
    Guid UomId,
    decimal Quantity,
    decimal? UnitCost);
