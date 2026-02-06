using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Commands.CreateInventoryCount;

public sealed class CreateInventoryCountCommandHandler
    : IRequestHandler<CreateInventoryCountCommand, RequestResult<InventoryCountDto>>
{
    private readonly IInventoryCountService _service;

    public CreateInventoryCountCommandHandler(IInventoryCountService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryCountDto>> Handle(CreateInventoryCountCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(request.WarehouseId, request.LocationId, request.ZoneId, request.Notes, cancellationToken);
}
