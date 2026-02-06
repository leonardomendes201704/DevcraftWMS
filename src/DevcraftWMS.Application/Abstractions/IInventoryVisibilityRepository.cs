using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IInventoryVisibilityRepository
{
    Task<IReadOnlyList<InventoryBalance>> ListBalancesAsync(
        Guid warehouseId,
        Guid? productId,
        string? sku,
        string? lotCode,
        DateOnly? expirationFrom,
        DateOnly? expirationTo,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
