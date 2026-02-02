using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.Structures;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class StructureServiceTests
{
    [Fact]
    public async Task CreateStructure_Should_Return_Failure_When_Section_Not_Found()
    {
        var structureRepository = new FakeStructureRepository();
        var sectionRepository = new FakeSectionRepository(null);
        var customerContext = new FakeCustomerContext();
        var service = new StructureService(structureRepository, sectionRepository, customerContext);

        var result = await service.CreateStructureAsync(
            Guid.NewGuid(),
            "R-01",
            "Rack",
            StructureType.SelectiveRack,
            4,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("structures.section.not_found");
    }

    [Fact]
    public async Task CreateStructure_Should_Return_Failure_When_Code_Exists()
    {
        var section = new Section { Id = Guid.NewGuid(), SectorId = Guid.NewGuid(), Code = "SEC-A", Name = "Section" };
        var structureRepository = new FakeStructureRepository(codeExists: true);
        var sectionRepository = new FakeSectionRepository(section);
        var customerContext = new FakeCustomerContext();
        var service = new StructureService(structureRepository, sectionRepository, customerContext);

        var result = await service.CreateStructureAsync(
            section.Id,
            "R-01",
            "Rack",
            StructureType.SelectiveRack,
            4,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("structures.structure.code_exists");
    }

    [Fact]
    public async Task UpdateStructure_Should_Return_Failure_When_Section_Mismatch()
    {
        var structure = new Structure
        {
            Id = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            Code = "R-01",
            Name = "Rack",
            StructureType = StructureType.SelectiveRack,
            Levels = 4
        };

        var structureRepository = new FakeStructureRepository(structure: structure);
        var sectionRepository = new FakeSectionRepository(new Section { Id = Guid.NewGuid(), SectorId = Guid.NewGuid(), Code = "SEC-A", Name = "Section" });
        var customerContext = new FakeCustomerContext();
        var service = new StructureService(structureRepository, sectionRepository, customerContext);

        var result = await service.UpdateStructureAsync(
            structure.Id,
            Guid.NewGuid(),
            "R-01",
            "Rack",
            StructureType.SelectiveRack,
            4,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("structures.section.mismatch");
    }

    private sealed class FakeStructureRepository : IStructureRepository
    {
        private readonly bool _codeExists;
        private readonly Structure? _structure;

        public FakeStructureRepository(bool codeExists = false, Structure? structure = null)
        {
            _codeExists = codeExists;
            _structure = structure;
        }

        public Task<bool> CodeExistsAsync(Guid sectionId, string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task<bool> CodeExistsAsync(Guid sectionId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task AddAsync(Structure structure, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(Structure structure, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Structure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_structure);

        public Task<Structure?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_structure);

        public Task<int> CountAsync(
            Guid sectionId,
            string? code,
            string? name,
            StructureType? structureType,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<Structure>> ListAsync(
            Guid sectionId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? name,
            StructureType? structureType,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Structure>>(Array.Empty<Structure>());
    }

    private sealed class FakeSectionRepository : ISectionRepository
    {
        private readonly Section? _section;

        public FakeSectionRepository(Section? section)
        {
            _section = section;
        }

        public Task<bool> CodeExistsAsync(Guid sectorId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> CodeExistsAsync(Guid sectorId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);

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

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public Guid? CustomerId { get; } = Guid.NewGuid();
    }
}
