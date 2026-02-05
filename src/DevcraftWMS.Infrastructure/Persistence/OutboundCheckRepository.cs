using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class OutboundCheckRepository : IOutboundCheckRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OutboundCheckRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboundCheck check, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboundChecks.Add(check);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OutboundCheck?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.OutboundChecks
            .AsNoTracking()
            .Include(x => x.OutboundOrder)
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(i => i.Product)
            .Include(x => x.Items)
                .ThenInclude(i => i.Uom)
            .Include(x => x.Items)
                .ThenInclude(i => i.Evidence)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
