using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Commands.CompleteInventoryCount;

public sealed class CompleteInventoryCountCommandHandler
    : IRequestHandler<CompleteInventoryCountCommand, RequestResult<InventoryCountDto>>
{
    private readonly IInventoryCountService _service;

    public CompleteInventoryCountCommandHandler(IInventoryCountService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryCountDto>> Handle(CompleteInventoryCountCommand request, CancellationToken cancellationToken)
        => _service.CompleteAsync(request.InventoryCountId, request.Items, request.Notes, cancellationToken);
}
