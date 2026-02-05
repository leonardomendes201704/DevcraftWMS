using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingTasks.Queries.GetPickingTaskById;

public sealed record GetPickingTaskByIdQuery(Guid Id)
    : IRequest<RequestResult<PickingTaskDetailDto>>;
