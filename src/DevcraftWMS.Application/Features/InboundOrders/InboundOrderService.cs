using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InboundOrders;

public sealed class InboundOrderService : IInboundOrderService
{
    private readonly IInboundOrderRepository _inboundOrderRepository;
    private readonly IAsnRepository _asnRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IPutawayTaskRepository _putawayTaskRepository;
    private readonly IUnitLoadRepository _unitLoadRepository;
    private readonly ICurrentUserService _currentUserService;

    public InboundOrderService(
        IInboundOrderRepository inboundOrderRepository,
        IAsnRepository asnRepository,
        IReceiptRepository receiptRepository,
        IPutawayTaskRepository putawayTaskRepository,
        IUnitLoadRepository unitLoadRepository,
        ICurrentUserService currentUserService)
    {
        _inboundOrderRepository = inboundOrderRepository;
        _asnRepository = asnRepository;
        _receiptRepository = receiptRepository;
        _putawayTaskRepository = putawayTaskRepository;
        _unitLoadRepository = unitLoadRepository;
        _currentUserService = currentUserService;
    }

    public async Task<RequestResult<PagedResult<InboundOrderListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? orderNumber,
        InboundOrderStatus? status,
        InboundOrderPriority? priority,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _inboundOrderRepository.CountAsync(
            warehouseId,
            orderNumber,
            status,
            priority,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _inboundOrderRepository.ListAsync(
            warehouseId,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            orderNumber,
            status,
            priority,
            isActive,
            includeInactive,
            cancellationToken);

        var mapped = items.Select(InboundOrderMapping.MapListItem).ToList();
        var result = new PagedResult<InboundOrderListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<InboundOrderListItemDto>>.Success(result);
    }

    public async Task<RequestResult<InboundOrderDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _inboundOrderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.not_found", "Inbound order not found.");
        }

        var items = order.Items.Select(InboundOrderMapping.MapItem).ToList();
        var statusEvents = order.StatusEvents
            .OrderByDescending(e => e.CreatedAtUtc)
            .Select(InboundOrderMapping.MapStatusEvent)
            .ToList();
        return RequestResult<InboundOrderDetailDto>.Success(InboundOrderMapping.MapDetail(order, items, statusEvents));
    }

    public async Task<RequestResult<InboundOrderDetailDto>> ConvertFromAsnAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
    {
        if (asnId == Guid.Empty)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.asn.required", "ASN is required.");
        }

        var asn = await _asnRepository.GetByIdAsync(asnId, cancellationToken);
        if (asn is null)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.asn.not_found", "ASN not found.");
        }

        if (asn.Status != AsnStatus.Approved)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.asn.not_approved", "ASN must be approved to convert.");
        }

        var exists = await _inboundOrderRepository.ExistsByAsnAsync(asnId, cancellationToken);
        if (exists)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.asn.already_converted", "ASN already converted to an inbound order.");
        }

        var orderNumber = await BuildOrderNumberAsync(asn.AsnNumber, cancellationToken);

        var order = new InboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = asn.CustomerId,
            WarehouseId = asn.WarehouseId,
            AsnId = asn.Id,
            OrderNumber = orderNumber,
            SupplierName = asn.SupplierName,
            DocumentNumber = asn.DocumentNumber,
            ExpectedArrivalDate = asn.ExpectedArrivalDate,
            Notes = string.IsNullOrWhiteSpace(notes) ? asn.Notes : notes.Trim(),
            Status = InboundOrderStatus.Issued,
            Priority = InboundOrderPriority.Normal,
            InspectionLevel = InboundOrderInspectionLevel.None
        };

        await _inboundOrderRepository.AddAsync(order, cancellationToken);

        var items = new List<InboundOrderItem>();
        foreach (var asnItem in asn.Items)
        {
            var item = new InboundOrderItem
            {
                Id = Guid.NewGuid(),
                InboundOrderId = order.Id,
                ProductId = asnItem.ProductId,
                UomId = asnItem.UomId,
                Quantity = asnItem.Quantity,
                LotCode = asnItem.LotCode,
                ExpirationDate = asnItem.ExpirationDate
            };

            await _inboundOrderRepository.AddItemAsync(item, cancellationToken);
            item.Product = asnItem.Product;
            item.Uom = asnItem.Uom;
            items.Add(item);
        }

        await _asnRepository.UpdateStatusAsync(asn.Id, AsnStatus.Converted, cancellationToken);
        await _asnRepository.AddStatusEventAsync(new AsnStatusEvent
        {
            Id = Guid.NewGuid(),
            AsnId = asn.Id,
            FromStatus = asn.Status,
            ToStatus = AsnStatus.Converted,
            Notes = "Converted to inbound order"
        }, cancellationToken);

        order.Asn = asn;
        order.Warehouse = asn.Warehouse;
        order.Items = items;

        var mappedItems = items.Select(InboundOrderMapping.MapItem).ToList();
        var mappedEvents = order.StatusEvents
            .OrderByDescending(e => e.CreatedAtUtc)
            .Select(InboundOrderMapping.MapStatusEvent)
            .ToList();
        return RequestResult<InboundOrderDetailDto>.Success(InboundOrderMapping.MapDetail(order, mappedItems, mappedEvents));
    }

    public async Task<RequestResult<InboundOrderDetailDto>> UpdateParametersAsync(
        Guid id,
        InboundOrderInspectionLevel inspectionLevel,
        InboundOrderPriority priority,
        string? suggestedDock,
        CancellationToken cancellationToken)
    {
        var order = await _inboundOrderRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.not_found", "Inbound order not found.");
        }

        if (order.Status is InboundOrderStatus.Canceled or InboundOrderStatus.Completed or InboundOrderStatus.PartiallyCompleted)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.status_locked", "Inbound order status does not allow updates.");
        }

        order.InspectionLevel = inspectionLevel;
        order.Priority = priority;
        order.SuggestedDock = string.IsNullOrWhiteSpace(suggestedDock) ? null : suggestedDock.Trim();

        await _inboundOrderRepository.UpdateAsync(order, cancellationToken);

        var items = await _inboundOrderRepository.ListItemsAsync(order.Id, cancellationToken);
        var mappedItems = items.Select(InboundOrderMapping.MapItem).ToList();
        var mappedEvents = order.StatusEvents
            .OrderByDescending(e => e.CreatedAtUtc)
            .Select(InboundOrderMapping.MapStatusEvent)
            .ToList();
        return RequestResult<InboundOrderDetailDto>.Success(InboundOrderMapping.MapDetail(order, mappedItems, mappedEvents));
    }

    public async Task<RequestResult<InboundOrderDetailDto>> CancelAsync(Guid id, string reason, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.cancel_reason_required", "Cancel reason is required.");
        }

        var order = await _inboundOrderRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.not_found", "Inbound order not found.");
        }

        if (order.Status is InboundOrderStatus.Completed or InboundOrderStatus.Canceled or InboundOrderStatus.PartiallyCompleted)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.status_locked", "Inbound order status does not allow cancellation.");
        }

        order.Status = InboundOrderStatus.Canceled;
        order.CancelReason = reason.Trim();
        order.CanceledAtUtc = DateTime.UtcNow;
        order.CanceledByUserId = _currentUserService.UserId;

        await _inboundOrderRepository.UpdateAsync(order, cancellationToken);

        var items = await _inboundOrderRepository.ListItemsAsync(order.Id, cancellationToken);
        var mappedItems = items.Select(InboundOrderMapping.MapItem).ToList();
        var mappedEvents = order.StatusEvents
            .OrderByDescending(e => e.CreatedAtUtc)
            .Select(InboundOrderMapping.MapStatusEvent)
            .ToList();
        return RequestResult<InboundOrderDetailDto>.Success(InboundOrderMapping.MapDetail(order, mappedItems, mappedEvents));
    }

    public async Task<RequestResult<InboundOrderDetailDto>> CompleteAsync(
        Guid id,
        bool allowPartial,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.required", "Inbound order is required.");
        }

        var order = await _inboundOrderRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.not_found", "Inbound order not found.");
        }

        if (order.Status is InboundOrderStatus.Canceled or InboundOrderStatus.Completed or InboundOrderStatus.PartiallyCompleted)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.status_locked", "Inbound order status does not allow completion.");
        }

        var receipts = await _receiptRepository.ListByInboundOrderIdAsync(order.Id, cancellationToken);
        if (receipts.Count == 0)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.receipts_required", "Inbound order requires at least one receipt.");
        }

        var hasIncompleteReceipts = receipts.Any(r => r.Status != ReceiptStatus.Completed);
        if (hasIncompleteReceipts && !allowPartial)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.receipts_not_completed", "All receipts must be completed before closing the inbound order.");
        }

        var receiptIds = receipts.Select(r => r.Id).ToArray();
        var hasPendingPutaway = await _putawayTaskRepository.AnyPendingByReceiptIdsAsync(receiptIds, cancellationToken);
        if (hasPendingPutaway && !allowPartial)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.putaway_incomplete", "Putaway tasks must be completed before closing the inbound order.");
        }

        var hasUnitLoadsPending = await _unitLoadRepository.AnyNotPutawayCompletedByReceiptIdsAsync(receiptIds, cancellationToken);
        if (hasUnitLoadsPending && !allowPartial)
        {
            return RequestResult<InboundOrderDetailDto>.Failure("inbound_orders.order.unit_loads_not_putaway", "All unit loads must be in a valid destination before closing the inbound order.");
        }

        var targetStatus = (hasIncompleteReceipts || hasPendingPutaway || hasUnitLoadsPending)
            ? InboundOrderStatus.PartiallyCompleted
            : InboundOrderStatus.Completed;

        var previousStatus = order.Status;
        order.Status = targetStatus;

        await _inboundOrderRepository.UpdateAsync(order, cancellationToken);

        var statusEvent = new InboundOrderStatusEvent
        {
            Id = Guid.NewGuid(),
            InboundOrderId = order.Id,
            FromStatus = previousStatus,
            ToStatus = targetStatus,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        await _inboundOrderRepository.AddStatusEventAsync(statusEvent, cancellationToken);

        var items = await _inboundOrderRepository.ListItemsAsync(order.Id, cancellationToken);
        order.StatusEvents.Add(statusEvent);

        var mappedItems = items.Select(InboundOrderMapping.MapItem).ToList();
        var mappedEvents = order.StatusEvents
            .OrderByDescending(e => e.CreatedAtUtc)
            .Select(InboundOrderMapping.MapStatusEvent)
            .ToList();

        return RequestResult<InboundOrderDetailDto>.Success(InboundOrderMapping.MapDetail(order, mappedItems, mappedEvents));
    }

    private async Task<string> BuildOrderNumberAsync(string asnNumber, CancellationToken cancellationToken)
    {
        var candidate = $"OE-{asnNumber}";
        var exists = await _inboundOrderRepository.OrderNumberExistsAsync(candidate, cancellationToken);
        if (!exists)
        {
            return candidate;
        }

        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var fallback = $"OE-{asnNumber}-{suffix}";
        return fallback.Length <= 32 ? fallback : fallback.Substring(0, 32);
    }
}
