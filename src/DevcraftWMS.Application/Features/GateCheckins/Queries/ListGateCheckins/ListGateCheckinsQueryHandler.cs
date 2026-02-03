using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.GateCheckins.Queries.ListGateCheckins;

public sealed class ListGateCheckinsQueryHandler : IRequestHandler<ListGateCheckinsQuery, RequestResult<PagedResult<GateCheckinListItemDto>>>
{
    private readonly IGateCheckinService _service;

    public ListGateCheckinsQueryHandler(IGateCheckinService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<GateCheckinListItemDto>>> Handle(ListGateCheckinsQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(
            request.InboundOrderId,
            request.DocumentNumber,
            request.VehiclePlate,
            request.DriverName,
            request.CarrierName,
            request.Status,
            request.ArrivalFromUtc,
            request.ArrivalToUtc,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
