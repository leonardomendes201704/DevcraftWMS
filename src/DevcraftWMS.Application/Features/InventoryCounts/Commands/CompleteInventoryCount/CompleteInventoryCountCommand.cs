using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Commands.CompleteInventoryCount;

public sealed record CompleteInventoryCountCommand(
    Guid InventoryCountId,
    IReadOnlyList<CompleteInventoryCountItemInput> Items,
    string? Notes)
    : IRequest<RequestResult<InventoryCountDto>>;
