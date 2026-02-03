using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.Asns;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class AsnServiceTests
{
    [Fact]
    public async Task CreateAsn_Should_Return_Failure_When_Customer_Context_Missing()
    {
        var service = new AsnService(
            new FakeAsnRepository(),
            new FakeAsnAttachmentRepository(),
            new FakeWarehouseRepository(new Warehouse { Id = Guid.NewGuid(), Name = "WH" }),
            new FakeCustomerContext(null));

        var result = await service.CreateAsync(
            Guid.NewGuid(),
            "ASN-001",
            null,
            null,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("customers.context.required");
    }

    [Fact]
    public async Task CreateAsn_Should_Return_Failure_When_Warehouse_Not_Found()
    {
        var service = new AsnService(
            new FakeAsnRepository(),
            new FakeAsnAttachmentRepository(),
            new FakeWarehouseRepository(null),
            new FakeCustomerContext(Guid.NewGuid()));

        var result = await service.CreateAsync(
            Guid.NewGuid(),
            "ASN-001",
            null,
            null,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("asns.warehouse.not_found");
    }

    [Fact]
    public async Task CreateAsn_Should_Return_Failure_When_AsnNumber_Exists()
    {
        var warehouseId = Guid.NewGuid();
        var service = new AsnService(
            new FakeAsnRepository(asnNumberExists: true),
            new FakeAsnAttachmentRepository(),
            new FakeWarehouseRepository(new Warehouse { Id = warehouseId, Name = "WH" }),
            new FakeCustomerContext(Guid.NewGuid()));

        var result = await service.CreateAsync(
            warehouseId,
            "ASN-001",
            null,
            null,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("asns.asn.number_exists");
    }

    private sealed class FakeAsnRepository : IAsnRepository
    {
        private readonly bool _asnNumberExists;

        public FakeAsnRepository(bool asnNumberExists = false)
        {
            _asnNumberExists = asnNumberExists;
        }

        public Task<bool> AsnNumberExistsAsync(string asnNumber, CancellationToken cancellationToken = default) => Task.FromResult(_asnNumberExists);
        public Task<bool> AsnNumberExistsAsync(string asnNumber, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_asnNumberExists);
        public Task AddAsync(Asn asn, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Asn asn, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Asn?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Asn?>(null);
        public Task<Asn?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Asn?>(null);
        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<int> CountAsync(Guid? warehouseId, string? asnNumber, string? supplierName, string? documentNumber, AsnStatus? status, DateOnly? expectedFrom, DateOnly? expectedTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Asn>> ListAsync(Guid? warehouseId, int pageNumber, int pageSize, string orderBy, string orderDir, string? asnNumber, string? supplierName, string? documentNumber, AsnStatus? status, DateOnly? expectedFrom, DateOnly? expectedTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Asn>>(Array.Empty<Asn>());
    }

    private sealed class FakeAsnAttachmentRepository : IAsnAttachmentRepository
    {
        public Task AddAsync(AsnAttachment attachment, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IReadOnlyList<AsnAttachment>> ListByAsnAsync(Guid asnId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<AsnAttachment>>(Array.Empty<AsnAttachment>());
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        private readonly Warehouse? _warehouse;

        public FakeWarehouseRepository(Warehouse? warehouse)
        {
            _warehouse = warehouse;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<int> CountAsync(string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public FakeCustomerContext(Guid? customerId)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; }
    }
}
