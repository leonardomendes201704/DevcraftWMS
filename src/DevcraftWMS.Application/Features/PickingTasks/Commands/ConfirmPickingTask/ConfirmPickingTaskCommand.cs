using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingTasks.Commands.ConfirmPickingTask;

public sealed record ConfirmPickingTaskItemInput(
    Guid PickingTaskItemId,
    decimal QuantityPicked);

public sealed record ConfirmPickingTaskCommand(
    Guid Id,
    IReadOnlyList<ConfirmPickingTaskItemInput> Items,
    string? Notes)
    : IRequest<RequestResult<PickingTaskDetailDto>>;
