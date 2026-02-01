using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Structures;

public interface IStructureService
{
    Task<RequestResult<StructureDto>> CreateStructureAsync(
        Guid sectionId,
        string code,
        string name,
        StructureType structureType,
        int levels,
        CancellationToken cancellationToken);

    Task<RequestResult<StructureDto>> UpdateStructureAsync(
        Guid id,
        Guid sectionId,
        string code,
        string name,
        StructureType structureType,
        int levels,
        CancellationToken cancellationToken);

    Task<RequestResult<StructureDto>> DeactivateStructureAsync(Guid id, CancellationToken cancellationToken);
}
