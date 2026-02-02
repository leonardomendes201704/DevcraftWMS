using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryMovements;

public interface IInventoryMovementService
{
    Task<RequestResult<InventoryMovementDto>> CreateAsync(
        Guid fromLocationId,
        Guid toLocationId,
        Guid productId,
        Guid? lotId,
        decimal quantity,
        string? reason,
        string? reference,
        DateTime? performedAtUtc,
        CancellationToken cancellationToken);

    Task<RequestResult<InventoryMovementDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<RequestResult<PagedResult<InventoryMovementListItemDto>>> ListAsync(
        Guid? productId,
        Guid? fromLocationId,
        Guid? toLocationId,
        Guid? lotId,
        InventoryMovementStatus? status,
        DateTime? performedFromUtc,
        DateTime? performedToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);
}
