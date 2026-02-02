using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.Locations;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class LocationServiceTests
{
    [Fact]
    public async Task CreateLocation_Should_Return_Failure_When_Structure_Not_Found()
    {
        var locationRepository = new FakeLocationRepository();
        var structureRepository = new FakeStructureRepository(null);
        var service = new LocationService(locationRepository, structureRepository);

        var result = await service.CreateLocationAsync(
            Guid.NewGuid(),
            "L-01",
            "BC-01",
            1,
            1,
            1,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("locations.structure.not_found");
    }

    [Fact]
    public async Task CreateLocation_Should_Return_Failure_When_Code_Exists()
    {
        var structure = new Structure { Id = Guid.NewGuid(), SectionId = Guid.NewGuid(), Code = "R-01", Name = "Rack", Levels = 4 };
        var locationRepository = new FakeLocationRepository(codeExists: true);
        var structureRepository = new FakeStructureRepository(structure);
        var service = new LocationService(locationRepository, structureRepository);

        var result = await service.CreateLocationAsync(
            structure.Id,
            "L-01",
            "BC-01",
            1,
            1,
            1,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("locations.location.code_exists");
    }

    [Fact]
    public async Task UpdateLocation_Should_Return_Failure_When_Structure_Mismatch()
    {
        var location = new Location
        {
            Id = Guid.NewGuid(),
            StructureId = Guid.NewGuid(),
            Code = "L-01",
            Barcode = "BC-01",
            Level = 1,
            Row = 1,
            Column = 1
        };

        var locationRepository = new FakeLocationRepository(location: location);
        var structureRepository = new FakeStructureRepository(new Structure { Id = Guid.NewGuid(), SectionId = Guid.NewGuid(), Code = "R-01", Name = "Rack", Levels = 4 });
        var service = new LocationService(locationRepository, structureRepository);

        var result = await service.UpdateLocationAsync(
            location.Id,
            Guid.NewGuid(),
            "L-01",
            "BC-01",
            1,
            1,
            1,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("locations.structure.mismatch");
    }

    private sealed class FakeLocationRepository : ILocationRepository
    {
        private readonly bool _codeExists;
        private readonly Location? _location;

        public FakeLocationRepository(bool codeExists = false, Location? location = null)
        {
            _codeExists = codeExists;
            _location = location;
        }

        public Task<bool> CodeExistsAsync(Guid structureId, string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task<bool> CodeExistsAsync(Guid structureId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task AddAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_location);

        public Task<Location?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_location);

        public Task<int> CountAsync(
            Guid structureId,
            string? code,
            string? barcode,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<Location>> ListAsync(
            Guid structureId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? barcode,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
    }

    private sealed class FakeStructureRepository : IStructureRepository
    {
        private readonly Structure? _structure;

        public FakeStructureRepository(Structure? structure)
        {
            _structure = structure;
        }

        public Task<bool> CodeExistsAsync(Guid sectionId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> CodeExistsAsync(Guid sectionId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);

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
}
