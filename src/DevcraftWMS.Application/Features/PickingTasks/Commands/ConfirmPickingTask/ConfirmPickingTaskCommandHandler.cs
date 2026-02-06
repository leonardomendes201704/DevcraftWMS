using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingTasks.Commands.ConfirmPickingTask;

public sealed class ConfirmPickingTaskCommandHandler
    : IRequestHandler<ConfirmPickingTaskCommand, RequestResult<PickingTaskDetailDto>>
{
    private readonly IPickingTaskRepository _repository;
    private readonly IOutboundOrderRepository _orderRepository;
    private readonly IOutboundCheckRepository _checkRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ConfirmPickingTaskCommandHandler(
        IPickingTaskRepository repository,
        IOutboundOrderRepository orderRepository,
        IOutboundCheckRepository checkRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _orderRepository = orderRepository;
        _checkRepository = checkRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<PickingTaskDetailDto>> Handle(ConfirmPickingTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetTrackedByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            return RequestResult<PickingTaskDetailDto>.Failure("picking.task.not_found", "Picking task not found.");
        }

        if (task.Status == PickingTaskStatus.Completed)
        {
            return RequestResult<PickingTaskDetailDto>.Success(PickingTaskMapping.MapDetail(task));
        }

        if (task.Status == PickingTaskStatus.Canceled)
        {
            return RequestResult<PickingTaskDetailDto>.Failure("picking.task.canceled", "Picking task is canceled.");
        }

        var now = _dateTimeProvider.UtcNow;
        if (!task.StartedAtUtc.HasValue)
        {
            task.StartedAtUtc = now;
        }

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            task.Notes = request.Notes.Trim();
        }

        var updatesById = request.Items.ToDictionary(i => i.PickingTaskItemId, i => i.QuantityPicked);
        foreach (var item in task.Items)
        {
            if (!updatesById.TryGetValue(item.Id, out var picked))
            {
                continue;
            }

            if (picked < 0)
            {
                return RequestResult<PickingTaskDetailDto>.Failure("picking.task.invalid_quantity", "Picked quantity cannot be negative.");
            }

            if (picked > item.QuantityPlanned)
            {
                return RequestResult<PickingTaskDetailDto>.Failure("picking.task.quantity_exceeded", "Picked quantity cannot exceed planned quantity.");
            }

            item.QuantityPicked = picked;
        }

        var completed = task.Items.All(i => i.QuantityPicked >= i.QuantityPlanned);
        task.Status = completed ? PickingTaskStatus.Completed : PickingTaskStatus.InProgress;
        if (completed)
        {
            task.CompletedAtUtc = now;
        }

        await _repository.UpdateAsync(task, cancellationToken);

        if (completed)
        {
            var totalTasks = await _repository.CountAsync(
                null,
                task.OutboundOrderId,
                null,
                null,
                true,
                false,
                cancellationToken);

            var completedTasks = await _repository.CountAsync(
                null,
                task.OutboundOrderId,
                null,
                PickingTaskStatus.Completed,
                true,
                false,
                cancellationToken);

            if (totalTasks > 0 && completedTasks == totalTasks)
            {
                var existingChecks = await _checkRepository.CountAsync(
                    null,
                    task.OutboundOrderId,
                    null,
                    null,
                    null,
                    false,
                    cancellationToken);

                if (existingChecks == 0)
                {
                    var order = await _orderRepository.GetTrackedByIdAsync(task.OutboundOrderId, cancellationToken);
                    if (order is not null)
                    {
                        await _checkRepository.AddAsync(new Domain.Entities.OutboundCheck
                        {
                            Id = Guid.NewGuid(),
                            CustomerId = order.CustomerId,
                            OutboundOrderId = order.Id,
                            WarehouseId = order.WarehouseId,
                            Status = Domain.Enums.OutboundCheckStatus.Pending,
                            Priority = order.Priority
                        }, cancellationToken);
                    }
                }
            }
        }

        return RequestResult<PickingTaskDetailDto>.Success(PickingTaskMapping.MapDetail(task));
    }
}
