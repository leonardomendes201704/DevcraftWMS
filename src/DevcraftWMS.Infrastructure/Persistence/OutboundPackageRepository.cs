using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class OutboundPackageRepository : IOutboundPackageRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OutboundPackageRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(IReadOnlyList<OutboundPackage> packages, CancellationToken cancellationToken = default)
    {
        if (packages.Count == 0)
        {
            return;
        }

        _dbContext.OutboundPackages.AddRange(packages);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutboundPackage>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default)
        => await _dbContext.OutboundPackages
            .AsNoTracking()
            .Include(p => p.OutboundOrder)
            .Include(p => p.Warehouse)
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .Include(p => p.Items)
                .ThenInclude(i => i.Uom)
            .Where(p => p.OutboundOrderId == outboundOrderId)
            .ToListAsync(cancellationToken);
}
