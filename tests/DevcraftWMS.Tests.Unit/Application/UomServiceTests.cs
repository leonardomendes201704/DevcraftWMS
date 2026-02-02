using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.Uoms;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class UomServiceTests
{
    [Fact]
    public async Task CreateUom_Should_Return_Failure_When_Code_Exists()
    {
        var repository = new FakeUomRepository(codeExists: true);
        var service = new UomService(repository);

        var result = await service.CreateUomAsync("UN", "Unit", UomType.Unit, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("uoms.uom.code_exists");
    }

    [Fact]
    public async Task UpdateUom_Should_Return_Failure_When_Not_Found()
    {
        var repository = new FakeUomRepository();
        var service = new UomService(repository);

        var result = await service.UpdateUomAsync(Guid.NewGuid(), "UN", "Unit", UomType.Unit, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("uoms.uom.not_found");
    }

    private sealed class FakeUomRepository : IUomRepository
    {
        private readonly bool _codeExists;
        private readonly Uom? _uom;

        public FakeUomRepository(bool codeExists = false, Uom? uom = null)
        {
            _codeExists = codeExists;
            _uom = uom;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);
        public Task AddAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Uom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<Uom?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<int> CountAsync(string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Uom>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Uom>>(Array.Empty<Uom>());
    }
}
