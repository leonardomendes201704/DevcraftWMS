using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.InventoryMovements.Queries.GetInventoryMovementById;

public sealed record GetInventoryMovementByIdQuery(Guid Id)
    : IRequest<RequestResult<InventoryMovementDto>>;
