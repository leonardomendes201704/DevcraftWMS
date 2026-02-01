using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sectors.Commands.DeactivateSector;

public sealed class DeactivateSectorCommandHandler : IRequestHandler<DeactivateSectorCommand, RequestResult<SectorDto>>
{
    private readonly ISectorService _sectorService;

    public DeactivateSectorCommandHandler(ISectorService sectorService)
    {
        _sectorService = sectorService;
    }

    public Task<RequestResult<SectorDto>> Handle(DeactivateSectorCommand request, CancellationToken cancellationToken)
        => _sectorService.DeactivateSectorAsync(request.Id, cancellationToken);
}
