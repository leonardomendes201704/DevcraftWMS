using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Commands.ReassignPutawayTask;

public sealed record ReassignPutawayTaskCommand(
    Guid Id,
    string AssigneeEmail,
    string Reason)
    : IRequest<RequestResult<PutawayTaskDetailDto>>;
