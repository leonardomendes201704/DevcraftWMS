using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingReplenishments.Commands.GeneratePickingReplenishments;

public sealed class GeneratePickingReplenishmentsCommandHandler
    : IRequestHandler<GeneratePickingReplenishmentsCommand, RequestResult<IReadOnlyList<PickingReplenishmentListItemDto>>>
{
    private readonly PickingReplenishmentService _service;

    public GeneratePickingReplenishmentsCommandHandler(PickingReplenishmentService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<PickingReplenishmentListItemDto>>> Handle(
        GeneratePickingReplenishmentsCommand request,
        CancellationToken cancellationToken)
        => _service.GenerateAsync(request.WarehouseId, cancellationToken);
}
