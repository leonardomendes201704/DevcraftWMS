using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Queries.GetAisleById;

public sealed class GetAisleByIdQueryHandler : IRequestHandler<GetAisleByIdQuery, RequestResult<AisleDto>>
{
    private readonly IAisleRepository _aisleRepository;

    public GetAisleByIdQueryHandler(IAisleRepository aisleRepository)
    {
        _aisleRepository = aisleRepository;
    }

    public async Task<RequestResult<AisleDto>> Handle(GetAisleByIdQuery request, CancellationToken cancellationToken)
    {
        var aisle = await _aisleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (aisle is null)
        {
            return RequestResult<AisleDto>.Failure("aisles.aisle.not_found", "Aisle not found.");
        }

        return RequestResult<AisleDto>.Success(AisleMapping.Map(aisle));
    }
}
