using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Queries.GetPutawayTaskById;

public sealed class GetPutawayTaskByIdQueryHandler : IRequestHandler<GetPutawayTaskByIdQuery, RequestResult<PutawayTaskDetailDto>>
{
    private readonly IPutawayTaskRepository _repository;

    public GetPutawayTaskByIdQueryHandler(IPutawayTaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PutawayTaskDetailDto>> Handle(GetPutawayTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("putaway.task.not_found", "Putaway task not found.");
        }

        return RequestResult<PutawayTaskDetailDto>.Success(PutawayTaskMapping.MapDetail(task));
    }
}
