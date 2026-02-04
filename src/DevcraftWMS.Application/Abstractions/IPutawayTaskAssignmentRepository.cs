using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IPutawayTaskAssignmentRepository
{
    Task AddAsync(PutawayTaskAssignmentEvent assignmentEvent, CancellationToken cancellationToken = default);
}
