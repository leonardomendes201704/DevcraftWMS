using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.UnitLoads;

public static class UnitLoadMapping
{
    public static UnitLoadListItemDto MapListItem(UnitLoad unitLoad)
        => new(
            unitLoad.Id,
            unitLoad.ReceiptId,
            unitLoad.Receipt?.ReceiptNumber ?? "-",
            unitLoad.WarehouseId,
            unitLoad.Warehouse?.Name ?? "-",
            unitLoad.SsccInternal,
            unitLoad.SsccExternal,
            unitLoad.Status,
            unitLoad.PrintedAtUtc,
            unitLoad.IsActive,
            unitLoad.CreatedAtUtc);

    public static UnitLoadDetailDto MapDetail(UnitLoad unitLoad)
        => new(
            unitLoad.Id,
            unitLoad.CustomerId,
            unitLoad.ReceiptId,
            unitLoad.Receipt?.ReceiptNumber ?? "-",
            unitLoad.WarehouseId,
            unitLoad.Warehouse?.Name ?? "-",
            unitLoad.SsccInternal,
            unitLoad.SsccExternal,
            unitLoad.Status,
            unitLoad.PrintedAtUtc,
            unitLoad.Notes,
            unitLoad.IsActive,
            unitLoad.CreatedAtUtc,
            unitLoad.UpdatedAtUtc);
}
