using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.Sections;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class SectionServiceTests
{
    [Fact]
    public async Task CreateSection_Should_Return_Failure_When_Sector_Not_Found()
    {
        var sectionRepository = new FakeSectionRepository();
        var sectorRepository = new FakeSectorRepository(null);
        var customerContext = new FakeCustomerContext();
        var service = new SectionService(sectionRepository, sectorRepository, customerContext);

        var result = await service.CreateSectionAsync(
            Guid.NewGuid(),
            "SEC-A",
            "Zone A",
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("sections.sector.not_found");
    }

    [Fact]
    public async Task CreateSection_Should_Return_Failure_When_Code_Exists()
    {
        var sector = new Sector { Id = Guid.NewGuid(), WarehouseId = Guid.NewGuid(), Code = "S-01", Name = "Sector" };
        var sectionRepository = new FakeSectionRepository(codeExists: true);
        var sectorRepository = new FakeSectorRepository(sector);
        var customerContext = new FakeCustomerContext();
        var service = new SectionService(sectionRepository, sectorRepository, customerContext);

        var result = await service.CreateSectionAsync(
            sector.Id,
            "SEC-A",
            "Zone A",
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("sections.section.code_exists");
    }

    [Fact]
    public async Task UpdateSection_Should_Return_Failure_When_Sector_Mismatch()
    {
        var section = new Section
        {
            Id = Guid.NewGuid(),
            SectorId = Guid.NewGuid(),
            Code = "SEC-A",
            Name = "Zone A"
        };

        var sectionRepository = new FakeSectionRepository(section: section);
        var sectorRepository = new FakeSectorRepository(new Sector { Id = Guid.NewGuid(), WarehouseId = Guid.NewGuid(), Code = "S-01", Name = "Sector" });
        var customerContext = new FakeCustomerContext();
        var service = new SectionService(sectionRepository, sectorRepository, customerContext);

        var result = await service.UpdateSectionAsync(
            section.Id,
            Guid.NewGuid(),
            "SEC-A",
            "Zone A",
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("sections.sector.mismatch");
    }

    private sealed class FakeSectionRepository : ISectionRepository
    {
        private readonly bool _codeExists;
        private readonly Section? _section;

        public FakeSectionRepository(bool codeExists = false, Section? section = null)
        {
            _codeExists = codeExists;
            _section = section;
        }

        public Task<bool> CodeExistsAsync(Guid sectorId, string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task<bool> CodeExistsAsync(Guid sectorId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task AddAsync(Section section, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(Section section, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Section?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_section);

        public Task<Section?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_section);

        public Task<int> CountAsync(
            Guid sectorId,
            string? code,
            string? name,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<Section>> ListAsync(
            Guid sectorId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? name,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Section>>(Array.Empty<Section>());
    }

    private sealed class FakeSectorRepository : ISectorRepository
    {
        private readonly Sector? _sector;

        public FakeSectorRepository(Sector? sector)
        {
            _sector = sector;
        }

        public Task<bool> CodeExistsAsync(Guid warehouseId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> CodeExistsAsync(Guid warehouseId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task AddAsync(Sector sector, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(Sector sector, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_sector);

        public Task<Sector?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_sector);

        public Task<int> CountAsync(
            Guid warehouseId,
            string? code,
            string? name,
            SectorType? sectorType,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<Sector>> ListAsync(
            Guid warehouseId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? name,
            SectorType? sectorType,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Sector>>(Array.Empty<Sector>());
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public Guid? CustomerId { get; } = Guid.NewGuid();
    }
}
