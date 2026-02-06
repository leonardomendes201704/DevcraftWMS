using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Commands.StartInventoryCount;

public sealed record StartInventoryCountCommand(Guid InventoryCountId)
    : IRequest<RequestResult<InventoryCountDto>>;
