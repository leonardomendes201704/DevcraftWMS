using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.DockSchedules;

public sealed class DockScheduleService : IDockScheduleService
{
    private readonly IDockScheduleRepository _dockRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IOutboundOrderRepository _orderRepository;
    private readonly IOutboundShipmentRepository _shipmentRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DockScheduleService(
        IDockScheduleRepository dockRepository,
        IWarehouseRepository warehouseRepository,
        IOutboundOrderRepository orderRepository,
        IOutboundShipmentRepository shipmentRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider)
    {
        _dockRepository = dockRepository;
        _warehouseRepository = warehouseRepository;
        _orderRepository = orderRepository;
        _shipmentRepository = shipmentRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<DockScheduleDto>> CreateAsync(
        Guid warehouseId,
        string dockCode,
        DateTime slotStartUtc,
        DateTime slotEndUtc,
        Guid? outboundOrderId,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<DockScheduleDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (warehouseId == Guid.Empty)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.warehouse.required", "Warehouse is required.");
        }

        if (string.IsNullOrWhiteSpace(dockCode))
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.dock_code.required", "Dock code is required.");
        }

        if (slotEndUtc <= slotStartUtc)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.slot.invalid", "Slot end must be after start.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.warehouse.not_found", "Warehouse not found.");
        }

        if (outboundOrderId.HasValue)
        {
            var order = await _orderRepository.GetByIdAsync(outboundOrderId.Value, cancellationToken);
            if (order is null)
            {
                return RequestResult<DockScheduleDto>.Failure("dock_schedule.order.not_found", "Outbound order not found.");
            }
        }

        var hasOverlap = await _dockRepository.HasOverlapAsync(warehouseId, dockCode.Trim(), slotStartUtc, slotEndUtc, null, cancellationToken);
        if (hasOverlap)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.slot.conflict", "Dock slot overlaps with an existing schedule.");
        }

        var schedule = new DockSchedule
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            WarehouseId = warehouseId,
            DockCode = dockCode.Trim(),
            SlotStartUtc = slotStartUtc,
            SlotEndUtc = slotEndUtc,
            Status = DockScheduleStatus.Scheduled,
            OutboundOrderId = outboundOrderId,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        await _dockRepository.AddAsync(schedule, cancellationToken);
        schedule.Warehouse = warehouse;
        return RequestResult<DockScheduleDto>.Success(DockScheduleMapping.MapDetail(schedule));
    }

    public async Task<RequestResult<DockScheduleDto>> RescheduleAsync(
        Guid scheduleId,
        DateTime slotStartUtc,
        DateTime slotEndUtc,
        string reason,
        CancellationToken cancellationToken)
    {
        if (scheduleId == Guid.Empty)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.schedule.required", "Dock schedule is required.");
        }

        if (slotEndUtc <= slotStartUtc)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.slot.invalid", "Slot end must be after start.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.reason.required", "Reschedule reason is required.");
        }

        var schedule = await _dockRepository.GetTrackedByIdAsync(scheduleId, cancellationToken);
        if (schedule is null)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.schedule.not_found", "Dock schedule not found.");
        }

        if (schedule.Status is DockScheduleStatus.Completed or DockScheduleStatus.Canceled)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.schedule.status_locked", "Dock schedule status does not allow reschedule.");
        }

        var hasOverlap = await _dockRepository.HasOverlapAsync(schedule.WarehouseId, schedule.DockCode, slotStartUtc, slotEndUtc, schedule.Id, cancellationToken);
        if (hasOverlap)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.slot.conflict", "Dock slot overlaps with an existing schedule.");
        }

        schedule.SlotStartUtc = slotStartUtc;
        schedule.SlotEndUtc = slotEndUtc;
        schedule.RescheduleReason = reason.Trim();

        await _dockRepository.UpdateAsync(schedule, cancellationToken);
        return RequestResult<DockScheduleDto>.Success(DockScheduleMapping.MapDetail(schedule));
    }

    public async Task<RequestResult<DockScheduleDto>> CancelAsync(Guid scheduleId, string reason, CancellationToken cancellationToken)
    {
        if (scheduleId == Guid.Empty)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.schedule.required", "Dock schedule is required.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.reason.required", "Cancel reason is required.");
        }

        var schedule = await _dockRepository.GetTrackedByIdAsync(scheduleId, cancellationToken);
        if (schedule is null)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.schedule.not_found", "Dock schedule not found.");
        }

        if (schedule.Status == DockScheduleStatus.Canceled)
        {
            return RequestResult<DockScheduleDto>.Success(DockScheduleMapping.MapDetail(schedule));
        }

        schedule.Status = DockScheduleStatus.Canceled;
        schedule.RescheduleReason = reason.Trim();

        await _dockRepository.UpdateAsync(schedule, cancellationToken);
        return RequestResult<DockScheduleDto>.Success(DockScheduleMapping.MapDetail(schedule));
    }

    public async Task<RequestResult<DockScheduleDto>> AssignAsync(Guid scheduleId, AssignDockScheduleInput input, CancellationToken cancellationToken)
    {
        if (scheduleId == Guid.Empty)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.schedule.required", "Dock schedule is required.");
        }

        if (!input.OutboundOrderId.HasValue && !input.OutboundShipmentId.HasValue)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.assignment.required", "Outbound order or shipment is required.");
        }

        var schedule = await _dockRepository.GetTrackedByIdAsync(scheduleId, cancellationToken);
        if (schedule is null)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.schedule.not_found", "Dock schedule not found.");
        }

        if (schedule.Status is DockScheduleStatus.Completed or DockScheduleStatus.Canceled)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.schedule.status_locked", "Dock schedule status does not allow assignment.");
        }

        if (input.OutboundOrderId.HasValue)
        {
            var order = await _orderRepository.GetByIdAsync(input.OutboundOrderId.Value, cancellationToken);
            if (order is null)
            {
                return RequestResult<DockScheduleDto>.Failure("dock_schedule.order.not_found", "Outbound order not found.");
            }

            schedule.OutboundOrderId = order.Id;
        }

        if (input.OutboundShipmentId.HasValue)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(input.OutboundShipmentId.Value, cancellationToken);
            if (shipment is null)
            {
                return RequestResult<DockScheduleDto>.Failure("dock_schedule.shipment.not_found", "Outbound shipment not found.");
            }

            schedule.OutboundShipmentId = shipment.Id;
        }

        schedule.Notes = string.IsNullOrWhiteSpace(input.Notes) ? schedule.Notes : input.Notes.Trim();
        await _dockRepository.UpdateAsync(schedule, cancellationToken);
        return RequestResult<DockScheduleDto>.Success(DockScheduleMapping.MapDetail(schedule));
    }

    public async Task<RequestResult<DockScheduleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var schedule = await _dockRepository.GetByIdAsync(id, cancellationToken);
        if (schedule is null)
        {
            return RequestResult<DockScheduleDto>.Failure("dock_schedule.schedule.not_found", "Dock schedule not found.");
        }

        return RequestResult<DockScheduleDto>.Success(DockScheduleMapping.MapDetail(schedule));
    }

    public async Task<RequestResult<PagedResult<DockScheduleListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? dockCode,
        DockScheduleStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _dockRepository.CountAsync(warehouseId, dockCode, status, fromUtc, toUtc, isActive, includeInactive, cancellationToken);
        var items = await _dockRepository.ListAsync(warehouseId, dockCode, status, fromUtc, toUtc, isActive, includeInactive, pageNumber, pageSize, orderBy, orderDir, cancellationToken);

        var mapped = items.Select(DockScheduleMapping.MapListItem).ToList();
        var result = new PagedResult<DockScheduleListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<DockScheduleListItemDto>>.Success(result);
    }
}
