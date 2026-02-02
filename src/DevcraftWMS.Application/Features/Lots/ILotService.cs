using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Lots;

public interface ILotService
{
    Task<RequestResult<LotDto>> CreateLotAsync(
        Guid productId,
        string code,
        DateOnly? manufactureDate,
        DateOnly? expirationDate,
        LotStatus status,
        CancellationToken cancellationToken);

    Task<RequestResult<LotDto>> UpdateLotAsync(
        Guid id,
        Guid productId,
        string code,
        DateOnly? manufactureDate,
        DateOnly? expirationDate,
        LotStatus status,
        CancellationToken cancellationToken);

    Task<RequestResult<LotDto>> DeactivateLotAsync(Guid id, CancellationToken cancellationToken);
}
