using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.InventoryVisibility;

public sealed record InventoryVisibilityQuery(
    Guid? CustomerId = null,
    Guid? WarehouseId = null,
    Guid? ProductId = null,
    string? Sku = null,
    string? LotCode = null,
    DateOnly? ExpirationFrom = null,
    DateOnly? ExpirationTo = null,
    InventoryBalanceStatus? Status = null,
    bool? IsActive = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "ProductCode",
    string OrderDir = "asc");

public sealed record InventoryVisibilitySummaryViewModel(
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string? UomCode,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    decimal QuantityBlocked,
    decimal QuantityInProcess,
    decimal QuantityAvailable);

public sealed record InventoryVisibilityLocationViewModel(
    Guid LocationId,
    string LocationCode,
    string? StructureCode,
    string? SectionCode,
    string? SectorCode,
    Guid WarehouseId,
    string? WarehouseName,
    Guid? ZoneId,
    string? ZoneCode,
    ZoneType? ZoneType,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string? LotCode,
    DateOnly? ExpirationDate,
    string? UnitLoadCode,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    InventoryBalanceStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record InventoryVisibilityTraceViewModel(
    string EventType,
    string Description,
    DateTime OccurredAtUtc,
    Guid? UserId);

public sealed record InventoryVisibilityResultViewModel(
    PagedResultDto<InventoryVisibilitySummaryViewModel> Summary,
    PagedResultDto<InventoryVisibilityLocationViewModel> Locations,
    IReadOnlyList<InventoryVisibilityTraceViewModel> Trace);

public sealed class InventoryVisibilityPageViewModel
{
    public InventoryVisibilityQuery Query { get; init; } = new();
    public IReadOnlyList<InventoryVisibilitySummaryViewModel> SummaryItems { get; init; } = Array.Empty<InventoryVisibilitySummaryViewModel>();
    public IReadOnlyList<InventoryVisibilityLocationViewModel> LocationItems { get; init; } = Array.Empty<InventoryVisibilityLocationViewModel>();
    public IReadOnlyList<InventoryVisibilityTraceViewModel> TraceItems { get; init; } = Array.Empty<InventoryVisibilityTraceViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Products { get; init; } = Array.Empty<SelectListItem>();
    public bool HasCustomerContext { get; init; }
}
