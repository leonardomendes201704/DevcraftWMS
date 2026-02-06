using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Commands.StartInventoryCount;

public sealed class StartInventoryCountCommandHandler
    : IRequestHandler<StartInventoryCountCommand, RequestResult<InventoryCountDto>>
{
    private readonly IInventoryCountService _service;

    public StartInventoryCountCommandHandler(IInventoryCountService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryCountDto>> Handle(StartInventoryCountCommand request, CancellationToken cancellationToken)
        => _service.StartAsync(request.InventoryCountId, cancellationToken);
}
