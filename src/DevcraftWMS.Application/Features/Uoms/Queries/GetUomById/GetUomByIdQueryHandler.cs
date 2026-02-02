using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Queries.GetUomById;

public sealed class GetUomByIdQueryHandler : IRequestHandler<GetUomByIdQuery, RequestResult<UomDto>>
{
    private readonly IUomRepository _uomRepository;

    public GetUomByIdQueryHandler(IUomRepository uomRepository)
    {
        _uomRepository = uomRepository;
    }

    public async Task<RequestResult<UomDto>> Handle(GetUomByIdQuery request, CancellationToken cancellationToken)
    {
        var uom = await _uomRepository.GetByIdAsync(request.Id, cancellationToken);
        if (uom is null)
        {
            return RequestResult<UomDto>.Failure("uoms.uom.not_found", "Unit of measure not found.");
        }

        return RequestResult<UomDto>.Success(UomMapping.Map(uom));
    }
}
