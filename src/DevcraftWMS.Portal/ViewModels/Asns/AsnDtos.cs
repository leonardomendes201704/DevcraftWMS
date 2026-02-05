namespace DevcraftWMS.Portal.ViewModels.Asns;

public sealed record AsnListItemDto(
    Guid Id,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    Guid WarehouseId,
    string WarehouseName,
    int Status,
    DateOnly? ExpectedArrivalDate,
    int ItemsCount,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record AsnDetailDto(
    Guid Id,
    Guid CustomerId,
    Guid WarehouseId,
    string WarehouseName,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    int Status,
    DateOnly? ExpectedArrivalDate,
    string? Notes,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record AsnCreateRequest(
    Guid WarehouseId,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    DateOnly? ExpectedArrivalDate,
    string? Notes);

public sealed record AsnAttachmentDto(
    Guid Id,
    Guid AsnId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string? StorageUrl,
    DateTime CreatedAtUtc);

public sealed record AsnItemDto(
    Guid Id,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid UomId,
    string UomCode,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record AsnStatusEventDto(
    Guid Id,
    Guid AsnId,
    int FromStatus,
    int ToStatus,
    string? Notes,
    DateTime CreatedAtUtc);
