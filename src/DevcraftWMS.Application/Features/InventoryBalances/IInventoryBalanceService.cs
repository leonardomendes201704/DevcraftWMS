using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryBalances;

public interface IInventoryBalanceService
{
    Task<RequestResult<InventoryBalanceDto>> CreateAsync(
        Guid locationId,
        Guid productId,
        Guid? lotId,
        decimal quantityOnHand,
        decimal quantityReserved,
        InventoryBalanceStatus status,
        CancellationToken cancellationToken);

    Task<RequestResult<InventoryBalanceDto>> UpdateAsync(
        Guid id,
        Guid locationId,
        Guid productId,
        Guid? lotId,
        decimal quantityOnHand,
        decimal quantityReserved,
        InventoryBalanceStatus status,
        CancellationToken cancellationToken);

    Task<RequestResult<InventoryBalanceDto>> DeactivateAsync(Guid id, CancellationToken cancellationToken);
}
