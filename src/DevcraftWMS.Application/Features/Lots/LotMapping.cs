using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Lots;

public static class LotMapping
{
    public static LotListItemDto MapListItem(Lot lot)
        => new(
            lot.Id,
            lot.ProductId,
            lot.Code,
            lot.ManufactureDate,
            lot.ExpirationDate,
            lot.Status,
            lot.IsActive,
            lot.CreatedAtUtc);

    public static LotDto Map(Lot lot)
        => new(
            lot.Id,
            lot.ProductId,
            lot.Code,
            lot.ManufactureDate,
            lot.ExpirationDate,
            lot.Status,
            lot.QuarantinedAtUtc,
            lot.QuarantineReason,
            lot.IsActive,
            lot.CreatedAtUtc);
}
