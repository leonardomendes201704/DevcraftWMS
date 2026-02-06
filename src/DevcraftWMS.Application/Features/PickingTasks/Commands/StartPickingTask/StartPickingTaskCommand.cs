using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingTasks.Commands.StartPickingTask;

public sealed record StartPickingTaskCommand(Guid Id) : IRequest<RequestResult<PickingTaskDetailDto>>;

public sealed class StartPickingTaskCommandHandler
    : IRequestHandler<StartPickingTaskCommand, RequestResult<PickingTaskDetailDto>>
{
    private readonly IPickingTaskRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public StartPickingTaskCommandHandler(
        IPickingTaskRepository repository,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<RequestResult<PickingTaskDetailDto>> Handle(StartPickingTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetTrackedByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            return RequestResult<PickingTaskDetailDto>.Failure("picking.task.not_found", "Picking task not found.");
        }

        if (task.Status == PickingTaskStatus.Completed)
        {
            return RequestResult<PickingTaskDetailDto>.Failure("picking.task.completed", "Picking task is completed.");
        }

        if (task.Status == PickingTaskStatus.Canceled)
        {
            return RequestResult<PickingTaskDetailDto>.Failure("picking.task.canceled", "Picking task is canceled.");
        }

        var currentUserId = _currentUserService.UserId;
        if (task.AssignedUserId.HasValue && task.AssignedUserId != currentUserId)
        {
            return RequestResult<PickingTaskDetailDto>.Failure("picking.task.assigned", "Picking task is already assigned to another user.");
        }

        if (task.Status == PickingTaskStatus.Pending)
        {
            task.Status = PickingTaskStatus.InProgress;
            task.AssignedUserId = currentUserId;
            task.StartedAtUtc ??= _dateTimeProvider.UtcNow;
            await _repository.UpdateAsync(task, cancellationToken);
        }

        return RequestResult<PickingTaskDetailDto>.Success(PickingTaskMapping.MapDetail(task));
    }
}
