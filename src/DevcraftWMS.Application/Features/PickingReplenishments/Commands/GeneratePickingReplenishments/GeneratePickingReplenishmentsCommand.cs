using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingReplenishments.Commands.GeneratePickingReplenishments;

public sealed record GeneratePickingReplenishmentsCommand(Guid? WarehouseId)
    : IRequest<RequestResult<IReadOnlyList<PickingReplenishmentListItemDto>>>;
