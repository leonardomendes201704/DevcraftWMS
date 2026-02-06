using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryVisibility.Queries.GetInventoryVisibilityTimeline;

public sealed record GetInventoryVisibilityTimelineQuery(
    Guid CustomerId,
    Guid WarehouseId,
    Guid ProductId,
    string? LotCode,
    Guid? LocationId)
    : IRequest<RequestResult<IReadOnlyList<InventoryVisibilityTraceDto>>>;
