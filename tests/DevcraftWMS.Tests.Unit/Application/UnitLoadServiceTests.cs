using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.UnitLoads;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class UnitLoadServiceTests
{
    [Fact]
    public async Task Create_Should_Return_Failure_When_Receipt_Not_Found()
    {
        var service = new UnitLoadService(
            new FakeUnitLoadRepository(),
            new FakeReceiptRepository(null),
            new FakeCustomerContext(),
            new FakeDateTimeProvider());

        var result = await service.CreateAsync(Guid.NewGuid(), null, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("unit_loads.receipt.not_found");
    }

    [Fact]
    public async Task Create_Should_Generate_Sscc_And_Status()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-UNIT-01",
            Status = ReceiptStatus.InProgress
        };

        var repository = new FakeUnitLoadRepository();
        var service = new UnitLoadService(
            repository,
            new FakeReceiptRepository(receipt),
            new FakeCustomerContext(),
            new FakeDateTimeProvider());

        var result = await service.CreateAsync(receipt.Id, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        repository.Stored.Should().ContainSingle();
        repository.Stored[0].Status.Should().Be(UnitLoadStatus.Created);
        repository.Stored[0].SsccInternal.Should().HaveLength(18);
    }

    [Fact]
    public async Task Print_Should_Update_Status_And_Return_Label()
    {
        var unitLoad = new UnitLoad
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptId = Guid.NewGuid(),
            SsccInternal = "251231235959999001",
            Status = UnitLoadStatus.Created
        };

        var repository = new FakeUnitLoadRepository(unitLoad);
        var service = new UnitLoadService(
            repository,
            new FakeReceiptRepository(null),
            new FakeCustomerContext(),
            new FakeDateTimeProvider());

        var result = await service.PrintLabelAsync(unitLoad.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Content.Should().Contain("SSCC:");
        repository.Stored[0].Status.Should().Be(UnitLoadStatus.Printed);
        repository.Stored[0].PrintedAtUtc.Should().NotBeNull();
    }

    private sealed class FakeUnitLoadRepository : IUnitLoadRepository
    {
        public List<UnitLoad> Stored { get; } = new();

        public FakeUnitLoadRepository(UnitLoad? initial = null)
        {
            if (initial is not null)
            {
                Stored.Add(initial);
            }
        }

        public Task AddAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default)
        {
            Stored.Add(unitLoad);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default)
        {
            var index = Stored.FindIndex(u => u.Id == unitLoad.Id);
            if (index >= 0)
            {
                Stored[index] = unitLoad;
            }
            else
            {
                Stored.Add(unitLoad);
            }
            return Task.CompletedTask;
        }

        public Task<UnitLoad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.SingleOrDefault(u => u.Id == id));

        public Task<UnitLoad?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.SingleOrDefault(u => u.Id == id));

        public Task<bool> SsccExistsAsync(string ssccInternal, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Any(u => u.SsccInternal == ssccInternal));

        public Task<int> CountAsync(Guid? warehouseId, Guid? receiptId, string? sscc, UnitLoadStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Count);

        public Task<IReadOnlyList<UnitLoad>> ListAsync(Guid? warehouseId, Guid? receiptId, string? sscc, UnitLoadStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<UnitLoad>>(Stored.ToList());
    }

    private sealed class FakeReceiptRepository : IReceiptRepository
    {
        private readonly Receipt? _receipt;

        public FakeReceiptRepository(Receipt? receipt)
        {
            _receipt = receipt;
        }

        public Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_receipt?.Id == id ? _receipt : null);
        public Task<Receipt?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_receipt?.Id == id ? _receipt : null);
        public Task<Receipt?> GetByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default) => Task.FromResult<Receipt?>(null);
        public Task<int> CountAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Receipt>> ListAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Receipt>>(Array.Empty<Receipt>());
        public Task AddItemAsync(ReceiptItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CountItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<ReceiptItem>> ListItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ReceiptItem>>(Array.Empty<ReceiptItem>());
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public Guid? CustomerId { get; } = Guid.NewGuid();
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => new DateTime(2026, 2, 3, 12, 0, 0, DateTimeKind.Utc);
    }
}
