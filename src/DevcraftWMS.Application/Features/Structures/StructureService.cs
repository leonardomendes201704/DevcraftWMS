using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Structures;

public sealed class StructureService : IStructureService
{
    private readonly IStructureRepository _structureRepository;
    private readonly ISectionRepository _sectionRepository;

    public StructureService(IStructureRepository structureRepository, ISectionRepository sectionRepository)
    {
        _structureRepository = structureRepository;
        _sectionRepository = sectionRepository;
    }

    public async Task<RequestResult<StructureDto>> CreateStructureAsync(
        Guid sectionId,
        string code,
        string name,
        StructureType structureType,
        int levels,
        CancellationToken cancellationToken)
    {
        var section = await _sectionRepository.GetByIdAsync(sectionId, cancellationToken);
        if (section is null)
        {
            return RequestResult<StructureDto>.Failure("structures.section.not_found", "Section not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var exists = await _structureRepository.CodeExistsAsync(sectionId, normalizedCode, cancellationToken);
        if (exists)
        {
            return RequestResult<StructureDto>.Failure("structures.structure.code_exists", "A structure with this code already exists.");
        }

        var structure = new Structure
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            Code = normalizedCode,
            Name = name.Trim(),
            StructureType = structureType,
            Levels = levels
        };

        await _structureRepository.AddAsync(structure, cancellationToken);
        return RequestResult<StructureDto>.Success(StructureMapping.Map(structure));
    }

    public async Task<RequestResult<StructureDto>> UpdateStructureAsync(
        Guid id,
        Guid sectionId,
        string code,
        string name,
        StructureType structureType,
        int levels,
        CancellationToken cancellationToken)
    {
        var structure = await _structureRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (structure is null)
        {
            return RequestResult<StructureDto>.Failure("structures.structure.not_found", "Structure not found.");
        }

        if (structure.SectionId != sectionId)
        {
            return RequestResult<StructureDto>.Failure("structures.section.mismatch", "Structure does not belong to the selected section.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(structure.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _structureRepository.CodeExistsAsync(sectionId, normalizedCode, id, cancellationToken);
            if (exists)
            {
                return RequestResult<StructureDto>.Failure("structures.structure.code_exists", "A structure with this code already exists.");
            }
        }

        structure.Code = normalizedCode;
        structure.Name = name.Trim();
        structure.StructureType = structureType;
        structure.Levels = levels;

        await _structureRepository.UpdateAsync(structure, cancellationToken);
        return RequestResult<StructureDto>.Success(StructureMapping.Map(structure));
    }

    public async Task<RequestResult<StructureDto>> DeactivateStructureAsync(Guid id, CancellationToken cancellationToken)
    {
        var structure = await _structureRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (structure is null)
        {
            return RequestResult<StructureDto>.Failure("structures.structure.not_found", "Structure not found.");
        }

        if (!structure.IsActive)
        {
            return RequestResult<StructureDto>.Success(StructureMapping.Map(structure));
        }

        structure.IsActive = false;
        await _structureRepository.UpdateAsync(structure, cancellationToken);
        return RequestResult<StructureDto>.Success(StructureMapping.Map(structure));
    }
}
