using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Queries.GetPutawayTaskById;

public sealed record GetPutawayTaskByIdQuery(Guid Id) : IRequest<RequestResult<PutawayTaskDetailDto>>;
