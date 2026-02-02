using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Aisles;

public interface IAisleService
{
    Task<RequestResult<AisleDto>> CreateAisleAsync(Guid sectionId, string code, string name, CancellationToken cancellationToken);
    Task<RequestResult<AisleDto>> UpdateAisleAsync(Guid id, Guid sectionId, string code, string name, CancellationToken cancellationToken);
    Task<RequestResult<AisleDto>> DeactivateAisleAsync(Guid id, CancellationToken cancellationToken);
}
