using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Commands.ConfirmPutawayTask;

public sealed record ConfirmPutawayTaskCommand(
    Guid Id,
    Guid LocationId,
    string? Notes)
    : IRequest<RequestResult<PutawayTaskDetailDto>>;
