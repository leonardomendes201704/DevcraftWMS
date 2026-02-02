using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Asns;

public sealed record AsnListItemDto(
    Guid Id,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    Guid WarehouseId,
    string WarehouseName,
    AsnStatus Status,
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
    AsnStatus Status,
    DateOnly? ExpectedArrivalDate,
    string? Notes,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

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
