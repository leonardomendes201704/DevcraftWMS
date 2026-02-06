using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundChecks.Queries.ListOutboundChecksPaged;

public sealed class ListOutboundChecksPagedQueryHandler
    : IRequestHandler<ListOutboundChecksPagedQuery, RequestResult<PagedResult<OutboundCheckListItemDto>>>
{
    private readonly IOutboundCheckRepository _repository;

    public ListOutboundChecksPagedQueryHandler(IOutboundCheckRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<OutboundCheckListItemDto>>> Handle(
        ListOutboundChecksPagedQuery request,
        CancellationToken cancellationToken)
    {
        var total = await _repository.CountAsync(
            request.WarehouseId,
            request.OutboundOrderId,
            request.Status,
            request.Priority,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _repository.ListAsync(
            request.WarehouseId,
            request.OutboundOrderId,
            request.Status,
            request.Priority,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);

        var mapped = items.Select(OutboundCheckMapping.MapListItem).ToList();
        var result = new PagedResult<OutboundCheckListItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<OutboundCheckListItemDto>>.Success(result);
    }
}
