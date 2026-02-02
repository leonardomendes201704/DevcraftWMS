using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.InventoryMovements.Commands.CreateInventoryMovement;

public sealed class CreateInventoryMovementCommandHandler
    : IRequestHandler<CreateInventoryMovementCommand, RequestResult<InventoryMovementDto>>
{
    private readonly IInventoryMovementService _service;

    public CreateInventoryMovementCommandHandler(IInventoryMovementService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryMovementDto>> Handle(CreateInventoryMovementCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(
            request.FromLocationId,
            request.ToLocationId,
            request.ProductId,
            request.LotId,
            request.Quantity,
            request.Reason,
            request.Reference,
            request.PerformedAtUtc,
            cancellationToken);
}
