using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Returns;

public interface IReturnService
{
    Task<RequestResult<ReturnOrderDto>> CreateAsync(
        Guid warehouseId,
        string returnNumber,
        Guid? outboundOrderId,
        string? notes,
        IReadOnlyList<CreateReturnItemInput> items,
        CancellationToken cancellationToken);
    Task<RequestResult<ReturnOrderDto>> ReceiveAsync(Guid returnOrderId, CancellationToken cancellationToken);
    Task<RequestResult<ReturnOrderDto>> CompleteAsync(
        Guid returnOrderId,
        IReadOnlyList<CompleteReturnItemInput> items,
        string? notes,
        CancellationToken cancellationToken);
    Task<RequestResult<ReturnOrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<RequestResult<PagedResult<ReturnOrderListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? returnNumber,
        ReturnStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);
}
