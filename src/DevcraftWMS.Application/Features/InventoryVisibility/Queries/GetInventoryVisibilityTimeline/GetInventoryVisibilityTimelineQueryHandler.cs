using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryVisibility.Queries.GetInventoryVisibilityTimeline;

public sealed class GetInventoryVisibilityTimelineQueryHandler
    : IRequestHandler<GetInventoryVisibilityTimelineQuery, RequestResult<IReadOnlyList<InventoryVisibilityTraceDto>>>
{
    private readonly IInventoryVisibilityService _service;

    public GetInventoryVisibilityTimelineQueryHandler(IInventoryVisibilityService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<InventoryVisibilityTraceDto>>> Handle(
        GetInventoryVisibilityTimelineQuery request,
        CancellationToken cancellationToken)
        => _service.GetTimelineAsync(
            request.CustomerId,
            request.WarehouseId,
            request.ProductId,
            request.LotCode,
            request.LocationId,
            cancellationToken);
}
