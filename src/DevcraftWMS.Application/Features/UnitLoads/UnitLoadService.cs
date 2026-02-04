using System.Text;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.UnitLoads;

public sealed class UnitLoadService : IUnitLoadService
{
    private const int MaxSsccAttempts = 5;

    private readonly IUnitLoadRepository _unitLoadRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IPutawayTaskRepository _putawayTaskRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UnitLoadService(
        IUnitLoadRepository unitLoadRepository,
        IReceiptRepository receiptRepository,
        IPutawayTaskRepository putawayTaskRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider)
    {
        _unitLoadRepository = unitLoadRepository;
        _receiptRepository = receiptRepository;
        _putawayTaskRepository = putawayTaskRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<UnitLoadDetailDto>> CreateAsync(
        Guid receiptId,
        string? ssccExternal,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<UnitLoadDetailDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var receipt = await _receiptRepository.GetByIdAsync(receiptId, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<UnitLoadDetailDto>.Failure("unit_loads.receipt.not_found", "Receipt not found.");
        }

        if (receipt.Status == ReceiptStatus.Canceled)
        {
            return RequestResult<UnitLoadDetailDto>.Failure("unit_loads.receipt.status_locked", "Receipt status does not allow unit load creation.");
        }

        var ssccInternal = await GenerateUniqueSsccAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(ssccInternal))
        {
            return RequestResult<UnitLoadDetailDto>.Failure("unit_loads.sscc.unavailable", "Unable to generate a unique SSCC.");
        }

        var unitLoad = new UnitLoad
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            WarehouseId = receipt.WarehouseId,
            ReceiptId = receipt.Id,
            SsccInternal = ssccInternal,
            SsccExternal = NormalizeOptional(ssccExternal),
            Notes = NormalizeOptional(notes),
            Status = UnitLoadStatus.Created
        };

        await _unitLoadRepository.AddAsync(unitLoad, cancellationToken);

        unitLoad.Receipt = receipt;
        unitLoad.Warehouse = receipt.Warehouse;
        return RequestResult<UnitLoadDetailDto>.Success(UnitLoadMapping.MapDetail(unitLoad));
    }

    public async Task<RequestResult<UnitLoadDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var unitLoad = await _unitLoadRepository.GetByIdAsync(id, cancellationToken);
        if (unitLoad is null)
        {
            return RequestResult<UnitLoadDetailDto>.Failure("unit_loads.unit_load.not_found", "Unit load not found.");
        }

        return RequestResult<UnitLoadDetailDto>.Success(UnitLoadMapping.MapDetail(unitLoad));
    }

    public async Task<RequestResult<PagedResult<UnitLoadListItemDto>>> ListAsync(
        Guid? warehouseId,
        Guid? receiptId,
        string? sscc,
        UnitLoadStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _unitLoadRepository.CountAsync(
            warehouseId,
            receiptId,
            sscc,
            status,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _unitLoadRepository.ListAsync(
            warehouseId,
            receiptId,
            sscc,
            status,
            isActive,
            includeInactive,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            cancellationToken);

        var mapped = items.Select(UnitLoadMapping.MapListItem).ToList();
        var result = new PagedResult<UnitLoadListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<UnitLoadListItemDto>>.Success(result);
    }

    public async Task<RequestResult<UnitLoadLabelDto>> PrintLabelAsync(Guid id, CancellationToken cancellationToken)
    {
        var unitLoad = await _unitLoadRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (unitLoad is null)
        {
            return RequestResult<UnitLoadLabelDto>.Failure("unit_loads.unit_load.not_found", "Unit load not found.");
        }

        if (unitLoad.Status == UnitLoadStatus.Canceled)
        {
            return RequestResult<UnitLoadLabelDto>.Failure("unit_loads.unit_load.status_locked", "Canceled unit loads cannot be printed.");
        }

        unitLoad.Status = UnitLoadStatus.Printed;
        unitLoad.PrintedAtUtc = _dateTimeProvider.UtcNow;
        await _unitLoadRepository.UpdateAsync(unitLoad, cancellationToken);

        await EnsurePutawayTaskAsync(unitLoad, cancellationToken);

        var printedAt = unitLoad.PrintedAtUtc ?? _dateTimeProvider.UtcNow;
        var receiptNumber = unitLoad.Receipt?.ReceiptNumber ?? "-";
        var warehouseName = unitLoad.Warehouse?.Name ?? "-";

        var labelContent = BuildLabelContent(unitLoad.SsccInternal, receiptNumber, warehouseName, printedAt);
        var dto = new UnitLoadLabelDto(
            unitLoad.Id,
            unitLoad.SsccInternal,
            receiptNumber,
            warehouseName,
            printedAt,
            labelContent);

        return RequestResult<UnitLoadLabelDto>.Success(dto);
    }

    private async Task EnsurePutawayTaskAsync(UnitLoad unitLoad, CancellationToken cancellationToken)
    {
        if (await _putawayTaskRepository.ExistsByUnitLoadIdAsync(unitLoad.Id, cancellationToken))
        {
            return;
        }

        var task = new PutawayTask
        {
            Id = Guid.NewGuid(),
            CustomerId = unitLoad.CustomerId,
            WarehouseId = unitLoad.WarehouseId,
            ReceiptId = unitLoad.ReceiptId,
            UnitLoadId = unitLoad.Id,
            Status = PutawayTaskStatus.Pending
        };

        await _putawayTaskRepository.AddAsync(task, cancellationToken);
    }

    private async Task<string?> GenerateUniqueSsccAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxSsccAttempts; attempt++)
        {
            var candidate = GenerateSsccCandidate();
            var exists = await _unitLoadRepository.SsccExistsAsync(candidate, cancellationToken);
            if (!exists)
            {
                return candidate;
            }
        }

        return null;
    }

    private static string GenerateSsccCandidate()
        => $"{DateTime.UtcNow:yyMMddHHmmssfff}{Random.Shared.Next(0, 1000):D3}";

    private static string BuildLabelContent(string sscc, string receiptNumber, string warehouseName, DateTime printedAtUtc)
    {
        var builder = new StringBuilder();
        builder.AppendLine("UNIT LOAD LABEL");
        builder.AppendLine($"SSCC: {sscc}");
        builder.AppendLine($"Receipt: {receiptNumber}");
        builder.AppendLine($"Warehouse: {warehouseName}");
        builder.AppendLine($"Printed (UTC): {printedAtUtc:yyyy-MM-dd HH:mm:ss}");
        return builder.ToString();
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
