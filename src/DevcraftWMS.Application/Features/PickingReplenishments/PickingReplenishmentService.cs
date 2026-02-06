using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Application.Features.PickingReplenishments;

public sealed class PickingReplenishmentService
{
    private readonly IPickingReplenishmentTaskRepository _taskRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICustomerContext _customerContext;
    private readonly PickingReplenishmentOptions _options;

    public PickingReplenishmentService(
        IPickingReplenishmentTaskRepository taskRepository,
        IInventoryBalanceRepository balanceRepository,
        IWarehouseRepository warehouseRepository,
        ICustomerContext customerContext,
        IOptions<PickingReplenishmentOptions> options)
    {
        _taskRepository = taskRepository;
        _balanceRepository = balanceRepository;
        _warehouseRepository = warehouseRepository;
        _customerContext = customerContext;
        _options = options.Value;
    }

    public async Task<RequestResult<PagedResult<PickingReplenishmentListItemDto>>> ListAsync(
        Guid? warehouseId,
        Guid? productId,
        PickingReplenishmentStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _taskRepository.CountAsync(
            warehouseId,
            productId,
            status,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _taskRepository.ListAsync(
            warehouseId,
            productId,
            status,
            isActive,
            includeInactive,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            cancellationToken);

        var mapped = items.Select(PickingReplenishmentMapping.MapListItem).ToList();
        var result = new PagedResult<PickingReplenishmentListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<PickingReplenishmentListItemDto>>.Success(result);
    }

    public async Task<RequestResult<IReadOnlyList<PickingReplenishmentListItemDto>>> GenerateAsync(
        Guid? warehouseId,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<IReadOnlyList<PickingReplenishmentListItemDto>>.Failure(
                "customers.context.required",
                "Customer context is required.");
        }

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId.Value, cancellationToken);
            if (warehouse is null)
            {
                return RequestResult<IReadOnlyList<PickingReplenishmentListItemDto>>.Failure(
                    "picking_replenishments.warehouse.not_found",
                    "Warehouse not found.");
            }
        }

        var pickingBalances = await _balanceRepository.ListByZonesAsync(
            new[] { ZoneType.Picking },
            cancellationToken);

        if (pickingBalances.Count == 0)
        {
            return RequestResult<IReadOnlyList<PickingReplenishmentListItemDto>>.Success(Array.Empty<PickingReplenishmentListItemDto>());
        }

        var storageBalances = await _balanceRepository.ListByZonesAsync(
            new[] { ZoneType.Storage, ZoneType.Bulk },
            cancellationToken);

        var tasks = new List<PickingReplenishmentTask>();
        var createdCount = 0;

        var pickingGroups = pickingBalances
            .Where(b => warehouseId == null || (b.Location?.Zone?.WarehouseId == warehouseId))
            .GroupBy(b => new { b.ProductId, b.LocationId })
            .ToList();

        foreach (var group in pickingGroups)
        {
            if (createdCount >= _options.MaxTasksPerRun)
            {
                break;
            }

            var available = group.Sum(b => b.QuantityOnHand - b.QuantityReserved);
            if (available >= _options.PickingMinQuantity)
            {
                continue;
            }

            var productId = group.Key.ProductId;
            var toLocationId = group.Key.LocationId;
            var location = group.First().Location;
            if (location is null)
            {
                continue;
            }

            var hasOpenTask = await _taskRepository.ExistsOpenTaskAsync(productId, toLocationId, cancellationToken);
            if (hasOpenTask)
            {
                continue;
            }

            var source = storageBalances
                .Where(b => b.ProductId == productId)
                .OrderByDescending(b => b.QuantityOnHand - b.QuantityReserved)
                .FirstOrDefault(b => (b.QuantityOnHand - b.QuantityReserved) > 0);

            if (source is null || source.LocationId == Guid.Empty)
            {
                continue;
            }

            var replenishQuantity = _options.PickingTargetQuantity - available;
            var sourceAvailable = source.QuantityOnHand - source.QuantityReserved;
            if (replenishQuantity <= 0 || sourceAvailable <= 0)
            {
                continue;
            }

            var quantityPlanned = Math.Min(replenishQuantity, sourceAvailable);
            var uomId = source.Product?.BaseUomId ?? Guid.Empty;
            if (uomId == Guid.Empty)
            {
                continue;
            }

            tasks.Add(new PickingReplenishmentTask
            {
                Id = Guid.NewGuid(),
                CustomerId = _customerContext.CustomerId.Value,
                WarehouseId = location.Zone?.WarehouseId ?? Guid.Empty,
                ProductId = productId,
                UomId = uomId,
                FromLocationId = source.LocationId,
                ToLocationId = toLocationId,
                QuantityPlanned = quantityPlanned,
                QuantityMoved = 0,
                Status = PickingReplenishmentStatus.Pending
            });

            createdCount++;
        }

        await _taskRepository.AddRangeAsync(tasks, cancellationToken);

        var mapped = tasks.Select(PickingReplenishmentMapping.MapListItem).ToList();
        return RequestResult<IReadOnlyList<PickingReplenishmentListItemDto>>.Success(mapped);
    }
}
