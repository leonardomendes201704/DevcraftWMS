using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
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
