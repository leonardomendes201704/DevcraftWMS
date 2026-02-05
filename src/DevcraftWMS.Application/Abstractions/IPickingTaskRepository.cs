using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IPickingTaskRepository
{
    Task AddRangeAsync(IReadOnlyList<PickingTask> tasks, CancellationToken cancellationToken = default);
}
