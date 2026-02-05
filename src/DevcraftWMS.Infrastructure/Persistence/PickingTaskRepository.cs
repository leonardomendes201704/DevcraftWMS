using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class PickingTaskRepository : IPickingTaskRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PickingTaskRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRangeAsync(IReadOnlyList<PickingTask> tasks, CancellationToken cancellationToken = default)
    {
        if (tasks.Count == 0)
        {
            return;
        }

        _dbContext.PickingTasks.AddRange(tasks);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
