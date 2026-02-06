using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.InventoryVisibility;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class InventoryVisibilityRepository : IInventoryVisibilityRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public InventoryVisibilityRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task<IReadOnlyList<InventoryBalance>> ListBalancesAsync(
        Guid warehouseId,
        Guid? productId,
        string? sku,
        string? lotCode,
        DateOnly? expirationFrom,
        DateOnly? expirationTo,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();

        var query = _dbContext.InventoryBalances
            .AsNoTracking()
            .Include(b => b.Product)
            .ThenInclude(p => p.BaseUom)
            .Include(b => b.Lot)
            .Include(b => b.Location)
            .ThenInclude(l => l.Zone)
            .Include(b => b.Location)
            .ThenInclude(l => l.Structure)
            .ThenInclude(s => s.Section)
            .ThenInclude(s => s.Sector)
            .ThenInclude(s => s.Warehouse)
            .Where(b => b.Product != null && b.Product.CustomerId == customerId)
            .Where(b => b.Location != null && b.Location.CustomerAccesses.Any(a => a.CustomerId == customerId))
            .Where(b => b.Location != null
                && b.Location.Structure != null
                && b.Location.Structure.Section != null
                && b.Location.Structure.Section.Sector != null
                && b.Location.Structure.Section.Sector.WarehouseId == warehouseId);

        if (productId.HasValue)
        {
            query = query.Where(b => b.ProductId == productId.Value);
        }

        if (!string.IsNullOrWhiteSpace(sku))
        {
            var normalized = sku.Trim().ToUpperInvariant();
            query = query.Where(b => b.Product != null
                && (b.Product.Code.ToUpperInvariant().Contains(normalized) || b.Product.Name.ToUpperInvariant().Contains(normalized)));
        }

        if (!string.IsNullOrWhiteSpace(lotCode))
        {
            var normalizedLot = lotCode.Trim().ToUpperInvariant();
            query = query.Where(b => b.Lot != null && b.Lot.Code.ToUpperInvariant().Contains(normalizedLot));
        }

        if (expirationFrom.HasValue)
        {
            query = query.Where(b => b.Lot != null && b.Lot.ExpirationDate.HasValue && b.Lot.ExpirationDate.Value >= expirationFrom.Value);
        }

        if (expirationTo.HasValue)
        {
            query = query.Where(b => b.Lot != null && b.Lot.ExpirationDate.HasValue && b.Lot.ExpirationDate.Value <= expirationTo.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(b => b.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(b => b.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryReservationSnapshot>> ListReservationsAsync(
        Guid warehouseId,
        IReadOnlyCollection<Guid> balanceIds,
        CancellationToken cancellationToken = default)
    {
        if (balanceIds.Count == 0)
        {
            return Array.Empty<InventoryReservationSnapshot>();
        }

        var query = from reservation in _dbContext.OutboundOrderReservations.AsNoTracking()
                    join balance in _dbContext.InventoryBalances.AsNoTracking()
                        on reservation.InventoryBalanceId equals balance.Id
                    join order in _dbContext.OutboundOrders.AsNoTracking()
                        on reservation.OutboundOrderId equals order.Id
                    where reservation.WarehouseId == warehouseId
                          && balanceIds.Contains(reservation.InventoryBalanceId)
                          && order.Status != OutboundOrderStatus.Canceled
                          && order.Status != OutboundOrderStatus.Shipped
                          && order.Status != OutboundOrderStatus.PartiallyShipped
                    select new
                    {
                        reservation.InventoryBalanceId,
                        reservation.ProductId,
                        reservation.LotId,
                        balance.LocationId,
                        reservation.QuantityReserved
                    };

        var rows = await query.ToListAsync(cancellationToken);
        return rows
            .GroupBy(r => new { r.InventoryBalanceId, r.ProductId, r.LotId, r.LocationId })
            .Select(g => new InventoryReservationSnapshot(
                g.Key.InventoryBalanceId,
                g.Key.ProductId,
                g.Key.LotId,
                g.Key.LocationId,
                g.Sum(x => x.QuantityReserved)))
            .ToList();
    }

    public async Task<IReadOnlyList<InventoryInspectionSnapshot>> ListBlockedInspectionsAsync(
        Guid warehouseId,
        IReadOnlyCollection<Guid> productIds,
        IReadOnlyCollection<Guid> locationIds,
        CancellationToken cancellationToken = default)
    {
        if (productIds.Count == 0 || locationIds.Count == 0)
        {
            return Array.Empty<InventoryInspectionSnapshot>();
        }

        var customerId = GetCustomerId();
        var query = from inspection in _dbContext.QualityInspections.AsNoTracking()
                    join receiptItem in _dbContext.ReceiptItems.AsNoTracking()
                        on inspection.ReceiptItemId equals receiptItem.Id into itemJoin
                    from receiptItem in itemJoin.DefaultIfEmpty()
                    where inspection.WarehouseId == warehouseId
                          && inspection.CustomerId == customerId
                          && inspection.Status != QualityInspectionStatus.Approved
                          && productIds.Contains(inspection.ProductId)
                          && locationIds.Contains(inspection.LocationId)
                    select new
                    {
                        inspection.ProductId,
                        inspection.LotId,
                        inspection.LocationId,
                        Quantity = receiptItem != null ? receiptItem.Quantity : 0m,
                        inspection.Status
                    };

        var rows = await query.ToListAsync(cancellationToken);
        return rows
            .GroupBy(r => new { r.ProductId, r.LotId, r.LocationId, r.Status })
            .Select(g => new InventoryInspectionSnapshot(
                g.Key.ProductId,
                g.Key.LotId,
                g.Key.LocationId,
                g.Sum(x => x.Quantity),
                g.Key.Status))
            .ToList();
    }

    public async Task<IReadOnlyList<InventoryInProcessSnapshot>> ListInProcessReceiptItemsAsync(
        Guid warehouseId,
        IReadOnlyCollection<Guid> productIds,
        IReadOnlyCollection<Guid> locationIds,
        CancellationToken cancellationToken = default)
    {
        if (productIds.Count == 0 || locationIds.Count == 0)
        {
            return Array.Empty<InventoryInProcessSnapshot>();
        }

        var customerId = GetCustomerId();
        var query = from receiptItem in _dbContext.ReceiptItems.AsNoTracking()
                    join receipt in _dbContext.Receipts.AsNoTracking()
                        on receiptItem.ReceiptId equals receipt.Id
                    where receipt.CustomerId == customerId
                          && receipt.WarehouseId == warehouseId
                          && (receipt.Status == ReceiptStatus.Draft || receipt.Status == ReceiptStatus.InProgress)
                          && productIds.Contains(receiptItem.ProductId)
                          && locationIds.Contains(receiptItem.LocationId)
                    select new
                    {
                        receiptItem.ProductId,
                        receiptItem.LotId,
                        receiptItem.LocationId,
                        receiptItem.Quantity
                    };

        var rows = await query.ToListAsync(cancellationToken);
        return rows
            .GroupBy(r => new { r.ProductId, r.LotId, r.LocationId })
            .Select(g => new InventoryInProcessSnapshot(
                g.Key.ProductId,
                g.Key.LotId,
                g.Key.LocationId,
                g.Sum(x => x.Quantity)))
            .ToList();
    }

    private Guid GetCustomerId()
    {
        var customerId = _customerContext.CustomerId;
        if (!customerId.HasValue)
        {
            throw new InvalidOperationException("Customer context is required.");
        }

        return customerId.Value;
    }
}
