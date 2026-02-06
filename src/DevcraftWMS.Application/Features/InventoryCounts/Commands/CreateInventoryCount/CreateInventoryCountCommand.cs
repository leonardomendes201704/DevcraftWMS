using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Commands.CreateInventoryCount;

public sealed record CreateInventoryCountCommand(
    Guid WarehouseId,
    Guid LocationId,
    Guid? ZoneId,
    string? Notes)
    : IRequest<RequestResult<InventoryCountDto>>;
