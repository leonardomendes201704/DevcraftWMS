using MediatR;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.CostCenters.Queries.GetCostCenterById;

public sealed class GetCostCenterByIdQueryHandler : IRequestHandler<GetCostCenterByIdQuery, RequestResult<CostCenterDto>>
{
    private readonly ICostCenterRepository _repository;

    public GetCostCenterByIdQueryHandler(ICostCenterRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<CostCenterDto>> Handle(GetCostCenterByIdQuery request, CancellationToken cancellationToken)
    {
        var costCenter = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return costCenter is null
            ? RequestResult<CostCenterDto>.Failure("cost_centers.cost_center.not_found", "Cost center not found.")
            : RequestResult<CostCenterDto>.Success(CostCenterMapping.Map(costCenter));
    }
}
