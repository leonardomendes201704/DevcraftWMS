using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Commands.ReassignPutawayTask;

public sealed class ReassignPutawayTaskCommandHandler
    : IRequestHandler<ReassignPutawayTaskCommand, RequestResult<PutawayTaskDetailDto>>
{
    private readonly IPutawayTaskRepository _taskRepository;
    private readonly IPutawayTaskAssignmentRepository _assignmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReassignPutawayTaskCommandHandler(
        IPutawayTaskRepository taskRepository,
        IPutawayTaskAssignmentRepository assignmentRepository,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _taskRepository = taskRepository;
        _assignmentRepository = assignmentRepository;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<PutawayTaskDetailDto>> Handle(ReassignPutawayTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetTrackedByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("putaway.task.not_found", "Putaway task not found.");
        }

        if (task.Status == PutawayTaskStatus.Completed)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("putaway.task.completed", "Completed putaway tasks cannot be reassigned.");
        }

        var assignee = await _userRepository.GetByEmailAsync(request.AssigneeEmail, cancellationToken);
        if (assignee is null)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("putaway.assignee.not_found", "Assignee user not found.");
        }

        var assignedAt = _dateTimeProvider.UtcNow;
        var assignmentEvent = new PutawayTaskAssignmentEvent
        {
            Id = Guid.NewGuid(),
            PutawayTaskId = task.Id,
            FromUserId = task.AssignedToUserId,
            FromUserEmail = task.AssignedToUserEmail,
            ToUserId = assignee.Id,
            ToUserEmail = assignee.Email,
            Reason = request.Reason.Trim(),
            AssignedAtUtc = assignedAt
        };

        task.AssignedToUserId = assignee.Id;
        task.AssignedToUserEmail = assignee.Email;

        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _assignmentRepository.AddAsync(assignmentEvent, cancellationToken);

        var refreshed = await _taskRepository.GetByIdAsync(task.Id, cancellationToken);
        if (refreshed is null)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("putaway.task.not_found", "Putaway task not found.");
        }

        return RequestResult<PutawayTaskDetailDto>.Success(PutawayTaskMapping.MapDetail(refreshed));
    }
}
