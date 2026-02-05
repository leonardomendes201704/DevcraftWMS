using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.PickingTasks.Commands.ConfirmPickingTask;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class ConfirmPickingTaskTests
{
    [Fact]
    public async Task Confirm_Should_Complete_When_All_Items_Picked()
    {
        var task = BuildTask();
        var now = new DateTime(2026, 2, 5, 12, 0, 0, DateTimeKind.Utc);
        var handler = new ConfirmPickingTaskCommandHandler(new FakePickingTaskRepository(task), new FakeDateTimeProvider(now));

        var command = new ConfirmPickingTaskCommand(task.Id, new List<ConfirmPickingTaskItemInput>
        {
            new(task.Items.ElementAt(0).Id, 3),
            new(task.Items.ElementAt(1).Id, 2)
        }, "Done");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        task.Status.Should().Be(PickingTaskStatus.Completed);
        task.CompletedAtUtc.Should().Be(now);
        task.Items.All(i => i.QuantityPicked >= i.QuantityPlanned).Should().BeTrue();
    }

    [Fact]
    public async Task Confirm_Should_Set_InProgress_When_Partial_Picked()
    {
        var task = BuildTask();
        var now = new DateTime(2026, 2, 5, 12, 0, 0, DateTimeKind.Utc);
        var handler = new ConfirmPickingTaskCommandHandler(new FakePickingTaskRepository(task), new FakeDateTimeProvider(now));

        var command = new ConfirmPickingTaskCommand(task.Id, new List<ConfirmPickingTaskItemInput>
        {
            new(task.Items.ElementAt(0).Id, 1),
            new(task.Items.ElementAt(1).Id, 0)
        }, null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        task.Status.Should().Be(PickingTaskStatus.InProgress);
        task.CompletedAtUtc.Should().BeNull();
    }

    [Fact]
    public async Task Confirm_Should_Fail_When_Quantity_Exceeds_Planned()
    {
        var task = BuildTask();
        var handler = new ConfirmPickingTaskCommandHandler(new FakePickingTaskRepository(task), new FakeDateTimeProvider(DateTime.UtcNow));

        var command = new ConfirmPickingTaskCommand(task.Id, new List<ConfirmPickingTaskItemInput>
        {
            new(task.Items.ElementAt(0).Id, 10)
        }, null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("picking.task.quantity_exceeded");
    }

    private static PickingTask BuildTask()
    {
        var task = new PickingTask
        {
            Id = Guid.NewGuid(),
            OutboundOrderId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Status = PickingTaskStatus.Pending,
            Sequence = 1,
            Items = new List<PickingTaskItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    PickingTaskId = Guid.NewGuid(),
                    OutboundOrderItemId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    UomId = Guid.NewGuid(),
                    QuantityPlanned = 3,
                    QuantityPicked = 0
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    PickingTaskId = Guid.NewGuid(),
                    OutboundOrderItemId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    UomId = Guid.NewGuid(),
                    QuantityPlanned = 2,
                    QuantityPicked = 0
                }
            }
        };

        return task;
    }

    private sealed class FakePickingTaskRepository : IPickingTaskRepository
    {
        private readonly PickingTask _task;

        public FakePickingTaskRepository(PickingTask task)
        {
            _task = task;
        }

        public Task AddRangeAsync(IReadOnlyList<PickingTask> tasks, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(PickingTask task, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<PickingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<PickingTask?>(id == _task.Id ? _task : null);
        public Task<PickingTask?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<PickingTask?>(id == _task.Id ? _task : null);
        public Task<int> CountAsync(Guid? warehouseId, Guid? outboundOrderId, Guid? assignedUserId, PickingTaskStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(1);
        public Task<IReadOnlyList<PickingTask>> ListAsync(Guid? warehouseId, Guid? outboundOrderId, Guid? assignedUserId, PickingTaskStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PickingTask>>(new[] { _task });
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }
}
