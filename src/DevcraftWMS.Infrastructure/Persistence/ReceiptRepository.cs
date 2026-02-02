using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class ReceiptRepository : IReceiptRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public ReceiptRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default)
    {
        _dbContext.Receipts.Add(receipt);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default)
    {
        _dbContext.Receipts.Update(receipt);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Receipts
            .AsNoTracking()
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
            .Where(r => r.CustomerId == customerId)
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Receipt?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Receipts
            .Include(r => r.Items)
            .Where(r => r.CustomerId == customerId)
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        string? receiptNumber,
        string? documentNumber,
        string? supplierName,
        ReceiptStatus? status,
        DateTime? receivedFromUtc,
        DateTime? receivedToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, receiptNumber, documentNumber, supplierName, status, receivedFromUtc, receivedToUtc, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Receipt>> ListAsync(
        Guid? warehouseId,
        string? receiptNumber,
        string? documentNumber,
        string? supplierName,
        ReceiptStatus? status,
        DateTime? receivedFromUtc,
        DateTime? receivedToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Receipt> query = BuildQuery(warehouseId, receiptNumber, documentNumber, supplierName, status, receivedFromUtc, receivedToUtc, isActive, includeInactive)
            .Include(r => r.Warehouse)
            .Include(r => r.Items);

        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddItemAsync(ReceiptItem item, CancellationToken cancellationToken = default)
    {
        _dbContext.ReceiptItems.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountItemsAsync(
        Guid receiptId,
        Guid? productId,
        Guid? locationId,
        Guid? lotId,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildItemsQuery(receiptId, productId, locationId, lotId, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReceiptItem>> ListItemsAsync(
        Guid receiptId,
        Guid? productId,
        Guid? locationId,
        Guid? lotId,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        var query = BuildItemsQuery(receiptId, productId, locationId, lotId, isActive, includeInactive);
        query = ApplyItemOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Receipt> BuildQuery(
        Guid? warehouseId,
        string? receiptNumber,
        string? documentNumber,
        string? supplierName,
        ReceiptStatus? status,
        DateTime? receivedFromUtc,
        DateTime? receivedToUtc,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.Receipts
            .AsNoTracking()
            .Where(r => r.CustomerId == customerId);

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(r => r.WarehouseId == warehouseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(receiptNumber))
        {
            query = query.Where(r => r.ReceiptNumber.Contains(receiptNumber));
        }

        if (!string.IsNullOrWhiteSpace(documentNumber))
        {
            query = query.Where(r => r.DocumentNumber != null && r.DocumentNumber.Contains(documentNumber));
        }

        if (!string.IsNullOrWhiteSpace(supplierName))
        {
            query = query.Where(r => r.SupplierName != null && r.SupplierName.Contains(supplierName));
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        if (receivedFromUtc.HasValue)
        {
            query = query.Where(r => r.ReceivedAtUtc >= receivedFromUtc.Value);
        }

        if (receivedToUtc.HasValue)
        {
            query = query.Where(r => r.ReceivedAtUtc <= receivedToUtc.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(r => r.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(r => r.IsActive);
        }

        return query;
    }

    private IQueryable<ReceiptItem> BuildItemsQuery(
        Guid receiptId,
        Guid? productId,
        Guid? locationId,
        Guid? lotId,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.ReceiptItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Include(i => i.Lot)
            .Include(i => i.Location)
            .Include(i => i.Uom)
            .Include(i => i.Receipt)
            .Where(i => i.ReceiptId == receiptId && i.Receipt != null && i.Receipt.CustomerId == customerId);

        if (productId.HasValue)
        {
            query = query.Where(i => i.ProductId == productId.Value);
        }

        if (locationId.HasValue)
        {
            query = query.Where(i => i.LocationId == locationId.Value);
        }

        if (lotId.HasValue)
        {
            query = query.Where(i => i.LotId == lotId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(i => i.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(i => i.IsActive);
        }

        return query;
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

    private static IQueryable<Receipt> ApplyOrdering(IQueryable<Receipt> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "receiptnumber" => asc ? query.OrderBy(r => r.ReceiptNumber) : query.OrderByDescending(r => r.ReceiptNumber),
            "documentnumber" => asc ? query.OrderBy(r => r.DocumentNumber) : query.OrderByDescending(r => r.DocumentNumber),
            "suppliername" => asc ? query.OrderBy(r => r.SupplierName) : query.OrderByDescending(r => r.SupplierName),
            "status" => asc ? query.OrderBy(r => r.Status) : query.OrderByDescending(r => r.Status),
            "receivedatutc" => asc ? query.OrderBy(r => r.ReceivedAtUtc) : query.OrderByDescending(r => r.ReceivedAtUtc),
            _ => asc ? query.OrderBy(r => r.CreatedAtUtc) : query.OrderByDescending(r => r.CreatedAtUtc)
        };
    }

    private static IQueryable<ReceiptItem> ApplyItemOrdering(IQueryable<ReceiptItem> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "productcode" => asc ? query.OrderBy(i => i.Product!.Code) : query.OrderByDescending(i => i.Product!.Code),
            "productname" => asc ? query.OrderBy(i => i.Product!.Name) : query.OrderByDescending(i => i.Product!.Name),
            "locationcode" => asc ? query.OrderBy(i => i.Location!.Code) : query.OrderByDescending(i => i.Location!.Code),
            "quantity" => asc ? query.OrderBy(i => i.Quantity) : query.OrderByDescending(i => i.Quantity),
            "uomcode" => asc ? query.OrderBy(i => i.Uom!.Code) : query.OrderByDescending(i => i.Uom!.Code),
            _ => asc ? query.OrderBy(i => i.CreatedAtUtc) : query.OrderByDescending(i => i.CreatedAtUtc)
        };
    }
}
