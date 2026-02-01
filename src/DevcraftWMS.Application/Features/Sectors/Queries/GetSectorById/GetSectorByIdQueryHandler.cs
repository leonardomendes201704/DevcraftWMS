using MediatR;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sectors.Queries.GetSectorById;

public sealed class GetSectorByIdQueryHandler : IRequestHandler<GetSectorByIdQuery, RequestResult<SectorDto>>
{
    private readonly ISectorRepository _sectorRepository;

    public GetSectorByIdQueryHandler(ISectorRepository sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public async Task<RequestResult<SectorDto>> Handle(GetSectorByIdQuery request, CancellationToken cancellationToken)
    {
        var sector = await _sectorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sector is null)
        {
            return RequestResult<SectorDto>.Failure("sectors.sector.not_found", "Sector not found.");
        }

        return RequestResult<SectorDto>.Success(SectorMapping.Map(sector));
    }
}
