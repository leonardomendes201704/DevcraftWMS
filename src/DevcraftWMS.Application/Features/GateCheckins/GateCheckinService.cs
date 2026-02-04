using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.GateCheckins;

public sealed class GateCheckinService : IGateCheckinService
{
    private readonly IGateCheckinRepository _gateCheckinRepository;
    private readonly IInboundOrderRepository _inboundOrderRepository;
    private readonly IAsnRepository _asnRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GateCheckinService(
        IGateCheckinRepository gateCheckinRepository,
        IInboundOrderRepository inboundOrderRepository,
        IAsnRepository asnRepository,
        IWarehouseRepository warehouseRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider)
    {
        _gateCheckinRepository = gateCheckinRepository;
        _inboundOrderRepository = inboundOrderRepository;
        _asnRepository = asnRepository;
        _warehouseRepository = warehouseRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<GateCheckinDetailDto>> CreateAsync(
        Guid? inboundOrderId,
        string? documentNumber,
        string vehiclePlate,
        string driverName,
        string? carrierName,
        DateTime? arrivalAtUtc,
        string? notes,
        Guid? warehouseId,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<GateCheckinDetailDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var inboundOrderResult = await ResolveInboundOrderAsync(inboundOrderId, documentNumber, warehouseId, cancellationToken);
        if (!inboundOrderResult.IsSuccess)
        {
            return RequestResult<GateCheckinDetailDto>.Failure(inboundOrderResult.ErrorCode!, inboundOrderResult.ErrorMessage!);
        }

        var inboundOrder = inboundOrderResult.Value;
        if (inboundOrder is null)
        {
            return RequestResult<GateCheckinDetailDto>.Failure("gate_checkins.inbound_order.required", "Inbound order or document number is required.");
        }

        var resolvedDocumentNumber = NormalizeOptional(documentNumber) ?? inboundOrder.DocumentNumber;
        var checkin = new GateCheckin
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            InboundOrderId = inboundOrder.Id,
            DocumentNumber = resolvedDocumentNumber,
            VehiclePlate = NormalizePlate(vehiclePlate),
            DriverName = driverName.Trim(),
            CarrierName = NormalizeOptional(carrierName),
            ArrivalAtUtc = arrivalAtUtc ?? _dateTimeProvider.UtcNow,
            Status = GateCheckinStatus.CheckedIn,
            Notes = NormalizeOptional(notes)
        };

        await _gateCheckinRepository.AddAsync(checkin, cancellationToken);
        checkin.InboundOrder = inboundOrder;
        return RequestResult<GateCheckinDetailDto>.Success(GateCheckinMapping.MapDetail(checkin));
    }

    public async Task<RequestResult<GateCheckinDetailDto>> UpdateAsync(
        Guid id,
        Guid? inboundOrderId,
        string? documentNumber,
        string vehiclePlate,
        string driverName,
        string? carrierName,
        DateTime arrivalAtUtc,
        GateCheckinStatus status,
        string? notes,
        CancellationToken cancellationToken)
    {
        var checkin = await _gateCheckinRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (checkin is null)
        {
            return RequestResult<GateCheckinDetailDto>.Failure("gate_checkins.checkin.not_found", "Gate check-in not found.");
        }

        var inboundOrderResult = await ResolveInboundOrderAsync(inboundOrderId, documentNumber, null, cancellationToken);
        if (!inboundOrderResult.IsSuccess)
        {
            return RequestResult<GateCheckinDetailDto>.Failure(inboundOrderResult.ErrorCode!, inboundOrderResult.ErrorMessage!);
        }

        var inboundOrder = inboundOrderResult.Value;
        if (inboundOrder is null)
        {
            return RequestResult<GateCheckinDetailDto>.Failure("gate_checkins.inbound_order.required", "Inbound order or document number is required.");
        }

        checkin.InboundOrderId = inboundOrder.Id;
        checkin.DocumentNumber = NormalizeOptional(documentNumber) ?? inboundOrder.DocumentNumber;
        checkin.VehiclePlate = NormalizePlate(vehiclePlate);
        checkin.DriverName = driverName.Trim();
        checkin.CarrierName = NormalizeOptional(carrierName);
        checkin.ArrivalAtUtc = arrivalAtUtc;
        checkin.Status = status;
        checkin.Notes = NormalizeOptional(notes);

        await _gateCheckinRepository.UpdateAsync(checkin, cancellationToken);
        checkin.InboundOrder = inboundOrder;
        return RequestResult<GateCheckinDetailDto>.Success(GateCheckinMapping.MapDetail(checkin));
    }

    public async Task<RequestResult<GateCheckinDetailDto>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var checkin = await _gateCheckinRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (checkin is null)
        {
            return RequestResult<GateCheckinDetailDto>.Failure("gate_checkins.checkin.not_found", "Gate check-in not found.");
        }

        if (!checkin.IsActive)
        {
            return RequestResult<GateCheckinDetailDto>.Success(GateCheckinMapping.MapDetail(checkin));
        }

        checkin.IsActive = false;
        await _gateCheckinRepository.UpdateAsync(checkin, cancellationToken);
        return RequestResult<GateCheckinDetailDto>.Success(GateCheckinMapping.MapDetail(checkin));
    }

    public async Task<RequestResult<GateCheckinDetailDto>> AssignDockAsync(
        Guid id,
        string dockCode,
        CancellationToken cancellationToken)
    {
        var checkin = await _gateCheckinRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (checkin is null)
        {
            return RequestResult<GateCheckinDetailDto>.Failure("gate_checkins.checkin.not_found", "Gate check-in not found.");
        }

        if (checkin.Status == GateCheckinStatus.Canceled)
        {
            return RequestResult<GateCheckinDetailDto>.Failure("gate_checkins.checkin.status_locked", "Canceled check-ins cannot be assigned to a dock.");
        }

        checkin.DockCode = dockCode.Trim();
        checkin.DockAssignedAtUtc = _dateTimeProvider.UtcNow;
        checkin.Status = GateCheckinStatus.AtDock;

        if (checkin.InboundOrderId.HasValue)
        {
            var inboundOrder = await _inboundOrderRepository.GetTrackedByIdAsync(checkin.InboundOrderId.Value, cancellationToken);
            if (inboundOrder is null)
            {
                return RequestResult<GateCheckinDetailDto>.Failure("gate_checkins.inbound_order.not_found", "Inbound order not found.");
            }

            if (inboundOrder.Status is InboundOrderStatus.Completed or InboundOrderStatus.Canceled or InboundOrderStatus.PartiallyCompleted)
            {
                return RequestResult<GateCheckinDetailDto>.Failure("gate_checkins.inbound_order.status_locked", "Inbound order status does not allow dock assignment.");
            }

            inboundOrder.SuggestedDock = checkin.DockCode;
            inboundOrder.Status = InboundOrderStatus.InProgress;
            await _inboundOrderRepository.UpdateAsync(inboundOrder, cancellationToken);
        }

        await _gateCheckinRepository.UpdateAsync(checkin, cancellationToken);
        return RequestResult<GateCheckinDetailDto>.Success(GateCheckinMapping.MapDetail(checkin));
    }

    public async Task<RequestResult<GateCheckinDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var checkin = await _gateCheckinRepository.GetByIdAsync(id, cancellationToken);
        if (checkin is null)
        {
            return RequestResult<GateCheckinDetailDto>.Failure("gate_checkins.checkin.not_found", "Gate check-in not found.");
        }

        return RequestResult<GateCheckinDetailDto>.Success(GateCheckinMapping.MapDetail(checkin));
    }

    public async Task<RequestResult<PagedResult<GateCheckinListItemDto>>> ListAsync(
        Guid? inboundOrderId,
        string? documentNumber,
        string? vehiclePlate,
        string? driverName,
        string? carrierName,
        GateCheckinStatus? status,
        DateTime? arrivalFromUtc,
        DateTime? arrivalToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _gateCheckinRepository.CountAsync(
            inboundOrderId,
            documentNumber,
            vehiclePlate,
            driverName,
            carrierName,
            status,
            arrivalFromUtc,
            arrivalToUtc,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _gateCheckinRepository.ListAsync(
            inboundOrderId,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            documentNumber,
            vehiclePlate,
            driverName,
            carrierName,
            status,
            arrivalFromUtc,
            arrivalToUtc,
            isActive,
            includeInactive,
            cancellationToken);

        var mapped = items.Select(GateCheckinMapping.MapListItem).ToList();
        var result = new PagedResult<GateCheckinListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<GateCheckinListItemDto>>.Success(result);
    }

    private async Task<RequestResult<InboundOrder?>> ResolveInboundOrderAsync(
        Guid? inboundOrderId,
        string? documentNumber,
        Guid? warehouseId,
        CancellationToken cancellationToken)
    {
        if (inboundOrderId.HasValue && inboundOrderId.Value != Guid.Empty)
        {
            var inboundOrder = await _inboundOrderRepository.GetByIdAsync(inboundOrderId.Value, cancellationToken);
            if (inboundOrder is null)
            {
                return RequestResult<InboundOrder?>.Failure("gate_checkins.inbound_order.not_found", "Inbound order not found.");
            }

            return RequestResult<InboundOrder?>.Success(inboundOrder);
        }

        if (!string.IsNullOrWhiteSpace(documentNumber))
        {
            var trimmedDocument = documentNumber.Trim();
            var inboundOrder = await _inboundOrderRepository.GetByDocumentNumberAsync(trimmedDocument, cancellationToken);
            if (inboundOrder is null)
            {
                if (!warehouseId.HasValue || warehouseId.Value == Guid.Empty)
                {
                    return RequestResult<InboundOrder?>.Failure("gate_checkins.warehouse.required", "Warehouse is required to create an emergency inbound order.");
                }

                var emergencyOrder = await CreateEmergencyInboundOrderAsync(trimmedDocument, warehouseId.Value, cancellationToken);
                return emergencyOrder;
            }

            return RequestResult<InboundOrder?>.Success(inboundOrder);
        }

        return RequestResult<InboundOrder?>.Success(null);
    }

    private async Task<RequestResult<InboundOrder?>> CreateEmergencyInboundOrderAsync(
        string documentNumber,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<InboundOrder?>.Failure("gate_checkins.warehouse.not_found", "Warehouse not found.");
        }

        var asnNumber = await BuildEmergencyAsnNumberAsync(cancellationToken);
        var asn = new Asn
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId!.Value,
            WarehouseId = warehouseId,
            AsnNumber = asnNumber,
            DocumentNumber = documentNumber,
            SupplierName = null,
            ExpectedArrivalDate = DateOnly.FromDateTime(_dateTimeProvider.UtcNow.Date),
            Notes = "Emergency ASN created from gate check-in.",
            Status = AsnStatus.Registered
        };

        await _asnRepository.AddAsync(asn, cancellationToken);

        var orderNumber = await BuildEmergencyOrderNumberAsync(asnNumber, cancellationToken);
        var order = new InboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId!.Value,
            WarehouseId = warehouseId,
            AsnId = asn.Id,
            OrderNumber = orderNumber,
            SupplierName = null,
            DocumentNumber = documentNumber,
            ExpectedArrivalDate = asn.ExpectedArrivalDate,
            Notes = "Emergency inbound order created from gate check-in.",
            Status = InboundOrderStatus.Scheduled,
            Priority = InboundOrderPriority.Normal,
            InspectionLevel = InboundOrderInspectionLevel.None,
            IsEmergency = true
        };

        await _inboundOrderRepository.AddAsync(order, cancellationToken);

        order.Asn = asn;
        order.Warehouse = warehouse;

        return RequestResult<InboundOrder?>.Success(order);
    }

    private async Task<string> BuildEmergencyAsnNumberAsync(CancellationToken cancellationToken)
    {
        var seed = _dateTimeProvider.UtcNow.ToString("yyyyMMddHHmmss");
        var candidate = $"ASN-EMG-{seed}";
        var exists = await _asnRepository.AsnNumberExistsAsync(candidate, cancellationToken);
        if (!exists)
        {
            return candidate;
        }

        var fallback = $"ASN-EMG-{seed}-{Guid.NewGuid():N}".Substring(0, 20);
        return fallback.Length <= 32 ? fallback : fallback.Substring(0, 32);
    }

    private async Task<string> BuildEmergencyOrderNumberAsync(string asnNumber, CancellationToken cancellationToken)
    {
        var candidate = $"OE-EMG-{asnNumber}";
        var exists = await _inboundOrderRepository.OrderNumberExistsAsync(candidate, cancellationToken);
        if (!exists)
        {
            return candidate.Length <= 32 ? candidate : candidate.Substring(0, 32);
        }

        var suffix = _dateTimeProvider.UtcNow.ToString("yyyyMMddHHmmss");
        var fallback = $"OE-EMG-{suffix}";
        return fallback.Length <= 32 ? fallback : fallback.Substring(0, 32);
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string NormalizePlate(string value)
        => value.Trim().ToUpperInvariant();
}
