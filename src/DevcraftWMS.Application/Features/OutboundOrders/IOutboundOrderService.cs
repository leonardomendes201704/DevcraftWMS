using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.OutboundOrders;

public interface IOutboundOrderService
{
    Task<RequestResult<OutboundOrderDetailDto>> CreateAsync(
        Guid warehouseId,
        string orderNumber,
        string? customerReference,
        string? carrierName,
        DateOnly? expectedShipDate,
        string? notes,
        IReadOnlyList<CreateOutboundOrderItemInput> items,
        CancellationToken cancellationToken);
    Task<RequestResult<OutboundOrderDetailDto>> ReleaseAsync(
        Guid id,
        OutboundOrderPriority priority,
        OutboundOrderPickingMethod pickingMethod,
        DateTime? shippingWindowStartUtc,
        DateTime? shippingWindowEndUtc,
        CancellationToken cancellationToken);

    Task<RequestResult<PagedResult<OutboundOrderListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? orderNumber,
        OutboundOrderStatus? status,
        OutboundOrderPriority? priority,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);

    Task<RequestResult<OutboundOrderDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}

public sealed record CreateOutboundOrderItemInput(
    Guid ProductId,
    Guid UomId,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate);
