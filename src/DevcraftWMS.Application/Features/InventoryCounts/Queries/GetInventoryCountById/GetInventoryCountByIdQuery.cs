using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Queries.GetInventoryCountById;

public sealed record GetInventoryCountByIdQuery(Guid InventoryCountId)
    : IRequest<RequestResult<InventoryCountDto>>;
