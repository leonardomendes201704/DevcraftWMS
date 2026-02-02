using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Queries.GetLotById;

public sealed class GetLotByIdQueryHandler : IRequestHandler<GetLotByIdQuery, RequestResult<LotDto>>
{
    private readonly ILotRepository _lotRepository;

    public GetLotByIdQueryHandler(ILotRepository lotRepository)
    {
        _lotRepository = lotRepository;
    }

    public async Task<RequestResult<LotDto>> Handle(GetLotByIdQuery request, CancellationToken cancellationToken)
    {
        var lot = await _lotRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lot is null)
        {
            return RequestResult<LotDto>.Failure("lots.lot.not_found", "Lot not found.");
        }

        return RequestResult<LotDto>.Success(LotMapping.Map(lot));
    }
}
