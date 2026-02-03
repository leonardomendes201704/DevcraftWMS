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
