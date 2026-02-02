using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Aisles;

public sealed class AisleService : IAisleService
{
    private readonly IAisleRepository _aisleRepository;
    private readonly ISectionRepository _sectionRepository;

    public AisleService(IAisleRepository aisleRepository, ISectionRepository sectionRepository)
    {
        _aisleRepository = aisleRepository;
        _sectionRepository = sectionRepository;
    }

    public async Task<RequestResult<AisleDto>> CreateAisleAsync(Guid sectionId, string code, string name, CancellationToken cancellationToken)
    {
        var section = await _sectionRepository.GetByIdAsync(sectionId, cancellationToken);
        if (section is null)
        {
            return RequestResult<AisleDto>.Failure("aisles.section.not_found", "Section not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var exists = await _aisleRepository.CodeExistsAsync(sectionId, normalizedCode, cancellationToken);
        if (exists)
        {
            return RequestResult<AisleDto>.Failure("aisles.aisle.code_exists", "An aisle with this code already exists.");
        }

        var aisle = new Aisle
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            Code = normalizedCode,
            Name = name.Trim()
        };

        await _aisleRepository.AddAsync(aisle, cancellationToken);
        return RequestResult<AisleDto>.Success(AisleMapping.Map(aisle));
    }

    public async Task<RequestResult<AisleDto>> UpdateAisleAsync(Guid id, Guid sectionId, string code, string name, CancellationToken cancellationToken)
    {
        var aisle = await _aisleRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (aisle is null)
        {
            return RequestResult<AisleDto>.Failure("aisles.aisle.not_found", "Aisle not found.");
        }

        if (aisle.SectionId != sectionId)
        {
            return RequestResult<AisleDto>.Failure("aisles.section.mismatch", "Aisle does not belong to the selected section.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(aisle.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _aisleRepository.CodeExistsAsync(sectionId, normalizedCode, id, cancellationToken);
            if (exists)
            {
                return RequestResult<AisleDto>.Failure("aisles.aisle.code_exists", "An aisle with this code already exists.");
            }
        }

        aisle.Code = normalizedCode;
        aisle.Name = name.Trim();

        await _aisleRepository.UpdateAsync(aisle, cancellationToken);
        return RequestResult<AisleDto>.Success(AisleMapping.Map(aisle));
    }

    public async Task<RequestResult<AisleDto>> DeactivateAisleAsync(Guid id, CancellationToken cancellationToken)
    {
        var aisle = await _aisleRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (aisle is null)
        {
            return RequestResult<AisleDto>.Failure("aisles.aisle.not_found", "Aisle not found.");
        }

        if (!aisle.IsActive)
        {
            return RequestResult<AisleDto>.Success(AisleMapping.Map(aisle));
        }

        aisle.IsActive = false;
        await _aisleRepository.UpdateAsync(aisle, cancellationToken);
        return RequestResult<AisleDto>.Success(AisleMapping.Map(aisle));
    }
}
