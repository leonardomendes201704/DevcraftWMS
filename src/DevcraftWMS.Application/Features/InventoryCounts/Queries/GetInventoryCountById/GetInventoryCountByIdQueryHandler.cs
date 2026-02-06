using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Queries.GetInventoryCountById;

public sealed class GetInventoryCountByIdQueryHandler
    : IRequestHandler<GetInventoryCountByIdQuery, RequestResult<InventoryCountDto>>
{
    private readonly IInventoryCountService _service;

    public GetInventoryCountByIdQueryHandler(IInventoryCountService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryCountDto>> Handle(GetInventoryCountByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.InventoryCountId, cancellationToken);
}
