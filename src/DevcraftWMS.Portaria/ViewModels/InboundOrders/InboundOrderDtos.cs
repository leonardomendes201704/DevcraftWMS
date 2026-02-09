namespace DevcraftWMS.Portaria.ViewModels.InboundOrders;

public sealed record InboundOrderListItemDto(
    Guid Id,
    string OrderNumber,
    string AsnNumber,
    string CustomerName,
    string WarehouseName,
    int Status,
    int Priority,
    DateOnly? ExpectedArrivalDate,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record InboundOrderListQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? WarehouseId = null,
    string? OrderNumber = null,
    int? Status = null,
    int? Priority = null,
    bool? IsActive = null,
    bool IncludeInactive = false);
