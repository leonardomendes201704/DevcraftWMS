using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sectors.Commands.CreateSector;

public sealed class CreateSectorCommandHandler : IRequestHandler<CreateSectorCommand, RequestResult<SectorDto>>
{
    private readonly ISectorService _sectorService;

    public CreateSectorCommandHandler(ISectorService sectorService)
    {
        _sectorService = sectorService;
    }

    public Task<RequestResult<SectorDto>> Handle(CreateSectorCommand request, CancellationToken cancellationToken)
        => _sectorService.CreateSectorAsync(
            request.WarehouseId,
            request.Code,
            request.Name,
            request.Description,
            request.SectorType,
            cancellationToken);
}
