using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Features.PutawayTasks.Commands.ReassignPutawayTask;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class ReassignPutawayTaskTests
{
    [Fact]
    public async Task Reassign_Should_Update_Assignee_And_Add_Event()
    {
        var task = new PutawayTask
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptId = Guid.NewGuid(),
            UnitLoadId = Guid.NewGuid(),
            Status = PutawayTaskStatus.Pending
        };

        var assignee = new User
        {
            Id = Guid.NewGuid(),
            Email = "assignee@test.local",
            FullName = "Assignee"
        };

        var assignments = new List<PutawayTaskAssignmentEvent>();
        var handler = new ReassignPutawayTaskCommandHandler(
            new FakePutawayTaskRepository(task, assignments),
            new FakeAssignmentRepository(assignments),
            new FakeUserRepository(assignee),
            new FakeDateTimeProvider());

        var result = await handler.Handle(new ReassignPutawayTaskCommand(task.Id, assignee.Email, "Reason"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        task.AssignedToUserId.Should().Be(assignee.Id);
        task.AssignedToUserEmail.Should().Be(assignee.Email);
        assignments.Should().HaveCount(1);
        assignments.Single().Reason.Should().Be("Reason");
    }

    [Fact]
    public async Task Reassign_Should_Fail_When_Task_Completed()
    {
        var task = new PutawayTask
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptId = Guid.NewGuid(),
            UnitLoadId = Guid.NewGuid(),
            Status = PutawayTaskStatus.Completed
        };

        var assignee = new User
        {
            Id = Guid.NewGuid(),
            Email = "assignee@test.local",
            FullName = "Assignee"
        };

        var assignments = new List<PutawayTaskAssignmentEvent>();
        var handler = new ReassignPutawayTaskCommandHandler(
            new FakePutawayTaskRepository(task, assignments),
            new FakeAssignmentRepository(assignments),
            new FakeUserRepository(assignee),
            new FakeDateTimeProvider());

        var result = await handler.Handle(new ReassignPutawayTaskCommand(task.Id, assignee.Email, "Reason"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("putaway.task.completed");
    }

    private sealed class FakePutawayTaskRepository : IPutawayTaskRepository
    {
        private readonly PutawayTask _task;
        private readonly List<PutawayTaskAssignmentEvent> _assignments;

        public FakePutawayTaskRepository(PutawayTask task, List<PutawayTaskAssignmentEvent> assignments)
        {
            _task = task;
            _assignments = assignments;
        }

        public Task AddAsync(PutawayTask task, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(PutawayTask task, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<PutawayTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id != _task.Id)
            {
                return Task.FromResult<PutawayTask?>(null);
            }

            _task.AssignmentEvents = _assignments.ToList();
            return Task.FromResult<PutawayTask?>(_task);
        }

        public Task<PutawayTask?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(id == _task.Id ? _task : null);
        public Task<bool> ExistsByUnitLoadIdAsync(Guid unitLoadId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> CountAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<IReadOnlyList<PutawayTask>> ListAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PutawayTask>>(new[] { _task });
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User _user;

        public FakeUserRepository(User user)
        {
            _user = user;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(email.Equals(_user.Email, StringComparison.OrdinalIgnoreCase) ? _user : null);

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(id == _user.Id ? _user : null);

        public Task AddAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeAssignmentRepository : IPutawayTaskAssignmentRepository
    {
        private readonly List<PutawayTaskAssignmentEvent> _assignments;

        public FakeAssignmentRepository(List<PutawayTaskAssignmentEvent> assignments)
        {
            _assignments = assignments;
        }

        public Task AddAsync(PutawayTaskAssignmentEvent assignmentEvent, CancellationToken cancellationToken = default)
        {
            _assignments.Add(assignmentEvent);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => new(2026, 2, 4, 10, 0, 0, DateTimeKind.Utc);
    }
}
