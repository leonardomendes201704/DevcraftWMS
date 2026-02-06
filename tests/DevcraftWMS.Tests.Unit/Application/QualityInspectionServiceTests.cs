using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.QualityInspections;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class QualityInspectionServiceTests
{
    [Fact]
    public async Task Approve_Should_Release_Lot_And_Balances()
    {
        var lot = new Lot
        {
            Id = Guid.NewGuid(),
            Status = LotStatus.Quarantined,
            QuarantinedAtUtc = DateTime.UtcNow.AddDays(-1)
        };

        var inspection = new QualityInspection
        {
            Id = Guid.NewGuid(),
            LotId = lot.Id,
            Status = QualityInspectionStatus.Pending,
            Reason = "Minimum shelf life not met"
        };

        var balance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LotId = lot.Id,
            Status = InventoryBalanceStatus.Blocked
        };

        var inspectionRepository = new FakeQualityInspectionRepository(inspection);
        var lotRepository = new FakeLotRepository(lot);
        var balanceRepository = new FakeInventoryBalanceRepository(balance);
        var dateTimeProvider = new FakeDateTimeProvider(new DateTime(2026, 2, 4, 10, 0, 0, DateTimeKind.Utc));
        var currentUserId = Guid.NewGuid();
        var currentUser = new FakeCurrentUserService(currentUserId);

        var service = new QualityInspectionService(
            inspectionRepository,
            lotRepository,
            balanceRepository,
            dateTimeProvider,
            currentUser);

        var result = await service.ApproveAsync(inspection.Id, "Approved", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        lot.Status.Should().Be(LotStatus.Available);
        balance.Status.Should().Be(InventoryBalanceStatus.Available);
        inspection.Status.Should().Be(QualityInspectionStatus.Approved);
        inspection.DecisionByUserId.Should().Be(currentUserId);
        inspection.DecisionAtUtc.Should().Be(dateTimeProvider.UtcNow);
    }

    [Fact]
    public async Task Reject_Should_Keep_Quarantine_And_Set_Status()
    {
        var lot = new Lot
        {
            Id = Guid.NewGuid(),
            Status = LotStatus.Quarantined,
            QuarantinedAtUtc = DateTime.UtcNow.AddDays(-1)
        };

        var inspection = new QualityInspection
        {
            Id = Guid.NewGuid(),
            LotId = lot.Id,
            Status = QualityInspectionStatus.Pending,
            Reason = "Minimum shelf life not met"
        };

        var inspectionRepository = new FakeQualityInspectionRepository(inspection);
        var lotRepository = new FakeLotRepository(lot);
        var balanceRepository = new FakeInventoryBalanceRepository();
        var dateTimeProvider = new FakeDateTimeProvider(new DateTime(2026, 2, 4, 11, 0, 0, DateTimeKind.Utc));
        var currentUserId = Guid.NewGuid();
        var currentUser = new FakeCurrentUserService(currentUserId);

        var service = new QualityInspectionService(
            inspectionRepository,
            lotRepository,
            balanceRepository,
            dateTimeProvider,
            currentUser);

        var result = await service.RejectAsync(inspection.Id, "Reject", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        inspection.Status.Should().Be(QualityInspectionStatus.Rejected);
        inspection.DecisionByUserId.Should().Be(currentUserId);
        lot.Status.Should().Be(LotStatus.Quarantined);
    }

    [Fact]
    public async Task Approve_Should_Fail_When_Status_Is_Not_Pending()
    {
        var inspection = new QualityInspection
        {
            Id = Guid.NewGuid(),
            Status = QualityInspectionStatus.Approved,
            Reason = "Already decided"
        };

        var service = new QualityInspectionService(
            new FakeQualityInspectionRepository(inspection),
            new FakeLotRepository(null),
            new FakeInventoryBalanceRepository(),
            new FakeDateTimeProvider(DateTime.UtcNow));

        var result = await service.ApproveAsync(inspection.Id, "", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("quality.inspection.status_locked");
    }

    private sealed class FakeQualityInspectionRepository : IQualityInspectionRepository
    {
        private readonly QualityInspection? _inspection;

        public FakeQualityInspectionRepository(QualityInspection? inspection)
        {
            _inspection = inspection;
        }

        public Task AddAsync(QualityInspection inspection, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(QualityInspection inspection, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<QualityInspection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_inspection?.Id == id ? _inspection : null);
        public Task<QualityInspection?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_inspection?.Id == id ? _inspection : null);
        public Task<bool> ExistsOpenForLotAsync(Guid lotId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> CountAsync(QualityInspectionStatus? status, Guid? warehouseId, Guid? receiptId, Guid? productId, Guid? lotId, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<QualityInspection>> ListAsync(QualityInspectionStatus? status, Guid? warehouseId, Guid? receiptId, Guid? productId, Guid? lotId, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<QualityInspection>>(Array.Empty<QualityInspection>());
        public Task AddEvidenceAsync(QualityInspectionEvidence evidence, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<QualityInspectionEvidence?> GetEvidenceByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<QualityInspectionEvidence?>(null);
        public Task<IReadOnlyList<QualityInspectionEvidence>> ListEvidenceAsync(Guid inspectionId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<QualityInspectionEvidence>>(Array.Empty<QualityInspectionEvidence>());
    }

    private sealed class FakeLotRepository : ILotRepository
    {
        private readonly Lot? _lot;

        public FakeLotRepository(Lot? lot)
        {
            _lot = lot;
        }

        public Task<bool> CodeExistsAsync(Guid productId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid productId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Lot lot, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Lot lot, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_lot?.Id == id ? _lot : null);
        public Task<Lot?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_lot?.Id == id ? _lot : null);
        public Task<Lot?> GetByCodeAsync(Guid productId, string code, CancellationToken cancellationToken = default) => Task.FromResult<Lot?>(null);
        public Task<int> CountAsync(Guid productId, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Lot>> ListAsync(Guid productId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Lot>>(Array.Empty<Lot>());
        public Task<int> CountExpiringAsync(DateOnly expirationFrom, DateOnly expirationTo, LotStatus? status, CancellationToken cancellationToken = default) => Task.FromResult(0);
    }

    private sealed class FakeInventoryBalanceRepository : IInventoryBalanceRepository
    {
        private readonly List<InventoryBalance> _balances = new();

        public FakeInventoryBalanceRepository(params InventoryBalance[] balances)
        {
            _balances.AddRange(balances);
        }

        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(InventoryBalance balance, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(InventoryBalance balance, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(null);
        public Task<InventoryBalance?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_balances.FirstOrDefault(b => b.Id == id));
        public Task<InventoryBalance?> GetTrackedByKeyAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(null);
        public Task<int> CountAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<int> CountByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<IReadOnlyList<InventoryBalance>> ListByLotAsync(Guid lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(_balances.Where(b => b.LotId == lotId).ToList());

        public Task<IReadOnlyList<InventoryBalance>> ListAvailableForReservationAsync(Guid productId, Guid? lotId, ZoneType? zoneType = null, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());

        public Task<IReadOnlyList<InventoryBalance>> ListByProductAndZonesAsync(Guid productId, IReadOnlyList<ZoneType> zoneTypes, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());

        public Task<IReadOnlyList<InventoryBalance>> ListByZonesAsync(IReadOnlyList<ZoneType> zoneTypes, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        public FakeCurrentUserService(Guid userId)
        {
            UserId = userId;
        }

        public Guid? UserId { get; }
        public string? Email => null;
    }
}
