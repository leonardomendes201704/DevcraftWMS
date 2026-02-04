using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class PutawayTaskAssignmentRepository : IPutawayTaskAssignmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PutawayTaskAssignmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PutawayTaskAssignmentEvent assignmentEvent, CancellationToken cancellationToken = default)
    {
        _dbContext.PutawayTaskAssignmentEvents.Add(assignmentEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
