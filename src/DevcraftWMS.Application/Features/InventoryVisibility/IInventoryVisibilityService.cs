using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.InventoryVisibility;

public interface IInventoryVisibilityService
{
    Task<RequestResult<InventoryVisibilityResultDto>> GetAsync(
        Guid customerId,
        Guid warehouseId,
        Guid? productId,
        string? sku,
        string? lotCode,
        DateOnly? expirationFrom,
        DateOnly? expirationTo,
        Domain.Enums.InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);

    Task<RequestResult<IReadOnlyList<InventoryVisibilityTraceDto>>> GetTimelineAsync(
        Guid customerId,
        Guid warehouseId,
        Guid productId,
        string? lotCode,
        Guid? locationId,
        CancellationToken cancellationToken);
}
