using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sections;

public interface ISectionService
{
    Task<RequestResult<SectionDto>> CreateSectionAsync(
        Guid sectorId,
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken);

    Task<RequestResult<SectionDto>> UpdateSectionAsync(
        Guid id,
        Guid sectorId,
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken);

    Task<RequestResult<SectionDto>> DeactivateSectionAsync(Guid id, CancellationToken cancellationToken);
}
