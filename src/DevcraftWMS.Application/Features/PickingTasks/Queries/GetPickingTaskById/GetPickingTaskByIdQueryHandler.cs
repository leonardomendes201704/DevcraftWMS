using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingTasks.Queries.GetPickingTaskById;

public sealed class GetPickingTaskByIdQueryHandler
    : IRequestHandler<GetPickingTaskByIdQuery, RequestResult<PickingTaskDetailDto>>
{
    private readonly IPickingTaskRepository _repository;

    public GetPickingTaskByIdQueryHandler(IPickingTaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PickingTaskDetailDto>> Handle(GetPickingTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            return RequestResult<PickingTaskDetailDto>.Failure("picking.task.not_found", "Picking task not found.");
        }

        return RequestResult<PickingTaskDetailDto>.Success(PickingTaskMapping.MapDetail(task));
    }
}
