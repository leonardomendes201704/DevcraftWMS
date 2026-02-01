using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Sections;

public sealed class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepository;
    private readonly ISectorRepository _sectorRepository;

    public SectionService(ISectionRepository sectionRepository, ISectorRepository sectorRepository)
    {
        _sectionRepository = sectionRepository;
        _sectorRepository = sectorRepository;
    }

    public async Task<RequestResult<SectionDto>> CreateSectionAsync(
        Guid sectorId,
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var sector = await _sectorRepository.GetByIdAsync(sectorId, cancellationToken);
        if (sector is null)
        {
            return RequestResult<SectionDto>.Failure("sections.sector.not_found", "Sector not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var exists = await _sectionRepository.CodeExistsAsync(sectorId, normalizedCode, cancellationToken);
        if (exists)
        {
            return RequestResult<SectionDto>.Failure("sections.section.code_exists", "A section with this code already exists.");
        }

        var section = new Section
        {
            Id = Guid.NewGuid(),
            SectorId = sectorId,
            Code = normalizedCode,
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
        };

        await _sectionRepository.AddAsync(section, cancellationToken);
        return RequestResult<SectionDto>.Success(SectionMapping.Map(section));
    }

    public async Task<RequestResult<SectionDto>> UpdateSectionAsync(
        Guid id,
        Guid sectorId,
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var section = await _sectionRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (section is null)
        {
            return RequestResult<SectionDto>.Failure("sections.section.not_found", "Section not found.");
        }

        if (section.SectorId != sectorId)
        {
            return RequestResult<SectionDto>.Failure("sections.sector.mismatch", "Section does not belong to the selected sector.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(section.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _sectionRepository.CodeExistsAsync(sectorId, normalizedCode, id, cancellationToken);
            if (exists)
            {
                return RequestResult<SectionDto>.Failure("sections.section.code_exists", "A section with this code already exists.");
            }
        }

        section.Code = normalizedCode;
        section.Name = name.Trim();
        section.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        await _sectionRepository.UpdateAsync(section, cancellationToken);
        return RequestResult<SectionDto>.Success(SectionMapping.Map(section));
    }

    public async Task<RequestResult<SectionDto>> DeactivateSectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var section = await _sectionRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (section is null)
        {
            return RequestResult<SectionDto>.Failure("sections.section.not_found", "Section not found.");
        }

        if (!section.IsActive)
        {
            return RequestResult<SectionDto>.Success(SectionMapping.Map(section));
        }

        section.IsActive = false;
        await _sectionRepository.UpdateAsync(section, cancellationToken);
        return RequestResult<SectionDto>.Success(SectionMapping.Map(section));
    }
}
