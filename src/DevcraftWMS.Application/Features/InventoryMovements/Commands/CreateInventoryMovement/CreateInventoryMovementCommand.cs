using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.InventoryMovements.Commands.CreateInventoryMovement;

public sealed record CreateInventoryMovementCommand(
    Guid FromLocationId,
    Guid ToLocationId,
    Guid ProductId,
    Guid? LotId,
    decimal Quantity,
    string? Reason,
    string? Reference,
    DateTime? PerformedAtUtc)
    : IRequest<RequestResult<InventoryMovementDto>>;
