using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.Aisles;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class AisleServiceTests
{
    [Fact]
    public async Task CreateAisle_Should_Return_Failure_When_Section_Not_Found()
    {
        var aisleRepository = new FakeAisleRepository();
        var sectionRepository = new FakeSectionRepository(null);
        var service = new AisleService(aisleRepository, sectionRepository);

        var result = await service.CreateAisleAsync(
            Guid.NewGuid(),
            "A-01",
            "Main Aisle",
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("aisles.section.not_found");
    }

    [Fact]
    public async Task CreateAisle_Should_Return_Failure_When_Code_Exists()
    {
        var section = new Section { Id = Guid.NewGuid(), SectorId = Guid.NewGuid(), Code = "SEC-A", Name = "Section" };
        var aisleRepository = new FakeAisleRepository(codeExists: true);
        var sectionRepository = new FakeSectionRepository(section);
        var service = new AisleService(aisleRepository, sectionRepository);

        var result = await service.CreateAisleAsync(
            section.Id,
            "A-01",
            "Main Aisle",
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("aisles.aisle.code_exists");
    }

    [Fact]
    public async Task UpdateAisle_Should_Return_Failure_When_Section_Mismatch()
    {
        var aisle = new Aisle
        {
            Id = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            Code = "A-01",
            Name = "Main"
        };

        var aisleRepository = new FakeAisleRepository(aisle: aisle);
        var sectionRepository = new FakeSectionRepository(new Section { Id = Guid.NewGuid(), SectorId = Guid.NewGuid(), Code = "SEC-A", Name = "Section" });
        var service = new AisleService(aisleRepository, sectionRepository);

        var result = await service.UpdateAisleAsync(
            aisle.Id,
            Guid.NewGuid(),
            "A-01",
            "Main",
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("aisles.section.mismatch");
    }

    private sealed class FakeAisleRepository : IAisleRepository
    {
        private readonly bool _codeExists;
        private readonly Aisle? _aisle;

        public FakeAisleRepository(bool codeExists = false, Aisle? aisle = null)
        {
            _codeExists = codeExists;
            _aisle = aisle;
        }

        public Task<bool> CodeExistsAsync(Guid sectionId, string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task<bool> CodeExistsAsync(Guid sectionId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task AddAsync(Aisle aisle, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(Aisle aisle, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Aisle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_aisle);

        public Task<Aisle?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_aisle);

        public Task<int> CountAsync(
            Guid sectionId,
            string? code,
            string? name,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<Aisle>> ListAsync(
            Guid sectionId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? name,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Aisle>>(Array.Empty<Aisle>());
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
}
