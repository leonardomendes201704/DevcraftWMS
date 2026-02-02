using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.InventoryMovements.Queries.GetInventoryMovementById;

public sealed class GetInventoryMovementByIdQueryHandler
    : IRequestHandler<GetInventoryMovementByIdQuery, RequestResult<InventoryMovementDto>>
{
    private readonly IInventoryMovementService _service;

    public GetInventoryMovementByIdQueryHandler(IInventoryMovementService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryMovementDto>> Handle(GetInventoryMovementByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
