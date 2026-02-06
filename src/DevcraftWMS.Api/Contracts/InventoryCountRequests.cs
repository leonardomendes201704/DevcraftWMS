namespace DevcraftWMS.Api.Contracts;

public sealed record CreateInventoryCountRequest(
    Guid WarehouseId,
    Guid LocationId,
    Guid? ZoneId,
    string? Notes);

public sealed record CompleteInventoryCountItemRequest(
    Guid InventoryCountItemId,
    decimal QuantityCounted);

public sealed record CompleteInventoryCountRequest(
    IReadOnlyList<CompleteInventoryCountItemRequest> Items,
    string? Notes);
