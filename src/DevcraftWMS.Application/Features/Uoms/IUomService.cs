using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Uoms;

public interface IUomService
{
    Task<RequestResult<UomDto>> CreateUomAsync(string code, string name, UomType type, CancellationToken cancellationToken);
    Task<RequestResult<UomDto>> UpdateUomAsync(Guid id, string code, string name, UomType type, CancellationToken cancellationToken);
    Task<RequestResult<UomDto>> DeactivateUomAsync(Guid id, CancellationToken cancellationToken);
}
