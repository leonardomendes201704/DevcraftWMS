using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.InventoryMovements;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Commands.ConfirmPutawayTask;

public sealed class ConfirmPutawayTaskCommandHandler
    : IRequestHandler<ConfirmPutawayTaskCommand, RequestResult<PutawayTaskDetailDto>>
{
    private readonly IPutawayTaskRepository _taskRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IUnitLoadRepository _unitLoadRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IInventoryMovementService _movementService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ConfirmPutawayTaskCommandHandler(
        IPutawayTaskRepository taskRepository,
        IReceiptRepository receiptRepository,
        IUnitLoadRepository unitLoadRepository,
        ILocationRepository locationRepository,
        IInventoryMovementService movementService,
        IDateTimeProvider dateTimeProvider)
    {
        _taskRepository = taskRepository;
        _receiptRepository = receiptRepository;
        _unitLoadRepository = unitLoadRepository;
        _locationRepository = locationRepository;
        _movementService = movementService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<PutawayTaskDetailDto>> Handle(ConfirmPutawayTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetTrackedByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("putaway.task.not_found", "Putaway task not found.");
        }

        if (task.Status == PutawayTaskStatus.Completed)
        {
            return RequestResult<PutawayTaskDetailDto>.Success(PutawayTaskMapping.MapDetail(task));
        }

        var receipt = await _receiptRepository.GetTrackedByIdAsync(task.ReceiptId, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("putaway.receipt.not_found", "Receipt not found.");
        }

        if (receipt.Status != ReceiptStatus.Completed)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("putaway.receipt.not_completed", "Receipt must be completed before putaway confirmation.");
        }

        var unitLoad = await _unitLoadRepository.GetTrackedByIdAsync(task.UnitLoadId, cancellationToken);
        if (unitLoad is null)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("unit_loads.unit_load.not_found", "Unit load not found.");
        }

        var targetLocation = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        if (targetLocation is null)
        {
            return RequestResult<PutawayTaskDetailDto>.Failure("putaway.location.not_found", "Target location not found.");
        }

        var performedAt = _dateTimeProvider.UtcNow;
        foreach (var item in receipt.Items)
        {
            if (item.LocationId == request.LocationId)
            {
                continue;
            }

            var movementResult = await _movementService.CreateAsync(
                item.LocationId,
                request.LocationId,
                item.ProductId,
                item.LotId,
                item.Quantity,
                request.Notes ?? "Putaway confirmation",
                $"PUTAWAY:{task.Id}",
                performedAt,
                cancellationToken);

            if (!movementResult.IsSuccess)
            {
                return RequestResult<PutawayTaskDetailDto>.Failure(
                    movementResult.ErrorCode ?? "putaway.movement.failed",
                    movementResult.ErrorMessage ?? "Failed to move inventory during putaway confirmation.");
            }
        }

        task.Status = PutawayTaskStatus.Completed;
        await _taskRepository.UpdateAsync(task, cancellationToken);

        unitLoad.Status = UnitLoadStatus.PutawayCompleted;
        await _unitLoadRepository.UpdateAsync(unitLoad, cancellationToken);

        return RequestResult<PutawayTaskDetailDto>.Success(PutawayTaskMapping.MapDetail(task));
    }
}
