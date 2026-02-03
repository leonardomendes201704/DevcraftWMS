using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InboundOrders;

public interface IInboundOrderService
{
    Task<RequestResult<PagedResult<InboundOrderListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? orderNumber,
        InboundOrderStatus? status,
        InboundOrderPriority? priority,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);

    Task<RequestResult<InboundOrderDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<RequestResult<InboundOrderDetailDto>> ConvertFromAsnAsync(Guid asnId, string? notes, CancellationToken cancellationToken);

    Task<RequestResult<InboundOrderDetailDto>> UpdateParametersAsync(
        Guid id,
        InboundOrderInspectionLevel inspectionLevel,
        InboundOrderPriority priority,
        string? suggestedDock,
        CancellationToken cancellationToken);

    Task<RequestResult<InboundOrderDetailDto>> CancelAsync(Guid id, string reason, CancellationToken cancellationToken);
}
