using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryCounts;

public interface IInventoryCountService
{
    Task<RequestResult<InventoryCountDto>> CreateAsync(
        Guid warehouseId,
        Guid locationId,
        Guid? zoneId,
        string? notes,
        CancellationToken cancellationToken);
    Task<RequestResult<InventoryCountDto>> StartAsync(Guid countId, CancellationToken cancellationToken);
    Task<RequestResult<InventoryCountDto>> CompleteAsync(
        Guid countId,
        IReadOnlyList<CompleteInventoryCountItemInput> items,
        string? notes,
        CancellationToken cancellationToken);
    Task<RequestResult<InventoryCountDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<RequestResult<PagedResult<InventoryCountListItemDto>>> ListAsync(
        Guid? warehouseId,
        Guid? locationId,
        InventoryCountStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);
}
