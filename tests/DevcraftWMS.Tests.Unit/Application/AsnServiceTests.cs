using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Abstractions.Storage;
using DevcraftWMS.Application.Features.Asns;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class AsnServiceTests
{
    [Fact]
    public async Task CreateAsn_Should_Return_Failure_When_Customer_Context_Missing()
    {
        var service = new AsnService(
            new FakeAsnRepository(),
            new FakeAsnAttachmentRepository(),
            new FakeAsnItemRepository(),
            new FakeWarehouseRepository(new Warehouse { Id = Guid.NewGuid(), Name = "WH" }),
            new FakeProductRepository(),
            new FakeUomRepository(),
            new FakeCustomerContext(null),
            new FakeFileStorage(),
            CreateOptions());

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
            new FakeAsnItemRepository(),
            new FakeWarehouseRepository(null),
            new FakeProductRepository(),
            new FakeUomRepository(),
            new FakeCustomerContext(Guid.NewGuid()),
            new FakeFileStorage(),
            CreateOptions());

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
            new FakeAsnItemRepository(),
            new FakeWarehouseRepository(new Warehouse { Id = warehouseId, Name = "WH" }),
            new FakeProductRepository(),
            new FakeUomRepository(),
            new FakeCustomerContext(Guid.NewGuid()),
            new FakeFileStorage(),
            CreateOptions());

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

    [Fact]
    public async Task AddItem_Should_Require_Lot_When_TrackingMode_Lot()
    {
        var asn = new Asn { Id = Guid.NewGuid(), Status = AsnStatus.Registered };
        var product = new Product { Id = Guid.NewGuid(), TrackingMode = TrackingMode.Lot };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA", Name = "Each", Type = UomType.Unit };

        var service = new AsnService(
            new FakeAsnRepository(asn: asn),
            new FakeAsnAttachmentRepository(),
            new FakeAsnItemRepository(),
            new FakeWarehouseRepository(new Warehouse { Id = Guid.NewGuid(), Name = "WH" }),
            new FakeProductRepository(product),
            new FakeUomRepository(uom),
            new FakeCustomerContext(Guid.NewGuid()),
            new FakeFileStorage(),
            CreateOptions());

        var result = await service.AddItemAsync(
            asn.Id,
            product.Id,
            uom.Id,
            10,
            lotCode: null,
            expirationDate: null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("asns.item.lot_required");
    }

    [Fact]
    public async Task AddItem_Should_Require_Lot_And_Expiry_When_TrackingMode_LotAndExpiry()
    {
        var asn = new Asn { Id = Guid.NewGuid(), Status = AsnStatus.Registered };
        var product = new Product { Id = Guid.NewGuid(), TrackingMode = TrackingMode.LotAndExpiry };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA", Name = "Each", Type = UomType.Unit };

        var service = new AsnService(
            new FakeAsnRepository(asn: asn),
            new FakeAsnAttachmentRepository(),
            new FakeAsnItemRepository(),
            new FakeWarehouseRepository(new Warehouse { Id = Guid.NewGuid(), Name = "WH" }),
            new FakeProductRepository(product),
            new FakeUomRepository(uom),
            new FakeCustomerContext(Guid.NewGuid()),
            new FakeFileStorage(),
            CreateOptions());

        var result = await service.AddItemAsync(
            asn.Id,
            product.Id,
            uom.Id,
            10,
            lotCode: "LOT-001",
            expirationDate: null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("asns.item.expiration_required");
    }

    private sealed class FakeAsnRepository : IAsnRepository
    {
        private readonly bool _asnNumberExists;
        private readonly Asn? _asn;

        public FakeAsnRepository(bool asnNumberExists = false, Asn? asn = null)
        {
            _asnNumberExists = asnNumberExists;
            _asn = asn;
        }

        public Task<bool> AsnNumberExistsAsync(string asnNumber, CancellationToken cancellationToken = default) => Task.FromResult(_asnNumberExists);
        public Task<bool> AsnNumberExistsAsync(string asnNumber, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_asnNumberExists);
        public Task AddAsync(Asn asn, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Asn asn, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> UpdateStatusAsync(Guid asnId, AsnStatus status, CancellationToken cancellationToken = default)
            => Task.FromResult(_asn?.Id == asnId);
        public Task AddStatusEventAsync(AsnStatusEvent statusEvent, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Asn?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_asn?.Id == id ? _asn : null);
        public Task<Asn?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_asn?.Id == id ? _asn : null);
        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_asn?.Id == id);
        public Task<int> CountAsync(Guid? warehouseId, string? asnNumber, string? supplierName, string? documentNumber, AsnStatus? status, DateOnly? expectedFrom, DateOnly? expectedTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Asn>> ListAsync(Guid? warehouseId, int pageNumber, int pageSize, string orderBy, string orderDir, string? asnNumber, string? supplierName, string? documentNumber, AsnStatus? status, DateOnly? expectedFrom, DateOnly? expectedTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Asn>>(Array.Empty<Asn>());
    }

    private sealed class FakeAsnAttachmentRepository : IAsnAttachmentRepository
    {
        public Task AddAsync(AsnAttachment attachment, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IReadOnlyList<AsnAttachment>> ListByAsnAsync(Guid asnId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<AsnAttachment>>(Array.Empty<AsnAttachment>());
        public Task<AsnAttachment?> GetByIdAsync(Guid asnId, Guid attachmentId, CancellationToken cancellationToken = default)
            => Task.FromResult<AsnAttachment?>(null);
    }

    private sealed class FakeAsnItemRepository : IAsnItemRepository
    {
        public Task AddAsync(AsnItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IReadOnlyList<AsnItem>> ListByAsnAsync(Guid asnId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<AsnItem>>(Array.Empty<AsnItem>());
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Product? _product;

        public FakeProductRepository(Product? product = null)
        {
            _product = product;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_product?.Id == id ? _product : null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_product?.Id == id ? _product : null);
        public Task<int> CountAsync(string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Product>>(Array.Empty<Product>());
    }

    private sealed class FakeUomRepository : IUomRepository
    {
        private readonly Uom? _uom;

        public FakeUomRepository(Uom? uom = null)
        {
            _uom = uom;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Uom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<Uom?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<int> CountAsync(string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Uom>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Uom>>(Array.Empty<Uom>());
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

    private sealed class FakeFileStorage : IFileStorage
    {
        public Task<FileStorageResult> SaveAsync(FileSaveRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new FileStorageResult("Database", null, null, Convert.ToBase64String(request.Content), "hash", request.Content.LongLength));

        public Task<FileReadResult?> ReadAsync(FileReadRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult<FileReadResult?>(null);
    }

    private static IOptions<FileStorageOptions> CreateOptions()
        => Options.Create(new FileStorageOptions
        {
            Provider = "Database",
            StoreContentBase64 = true,
            MaxFileSizeBytes = 10_000_000,
            AllowedContentTypes = Array.Empty<string>(),
            AsnAttachmentsPath = "asns"
        });
}
