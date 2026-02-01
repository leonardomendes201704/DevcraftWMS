using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sectors.Commands.UpdateSector;

public sealed class UpdateSectorCommandHandler : IRequestHandler<UpdateSectorCommand, RequestResult<SectorDto>>
{
    private readonly ISectorService _sectorService;

    public UpdateSectorCommandHandler(ISectorService sectorService)
    {
        _sectorService = sectorService;
    }

    public Task<RequestResult<SectorDto>> Handle(UpdateSectorCommand request, CancellationToken cancellationToken)
        => _sectorService.UpdateSectorAsync(
            request.Id,
            request.WarehouseId,
            request.Code,
            request.Name,
            request.Description,
            request.SectorType,
            cancellationToken);
}
