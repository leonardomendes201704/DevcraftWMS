using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundPacking.Queries.ListOutboundPackages;

public sealed class ListOutboundPackagesQueryHandler
    : IRequestHandler<ListOutboundPackagesQuery, RequestResult<IReadOnlyList<OutboundPackageDto>>>
{
    private readonly IOutboundPackageRepository _packageRepository;

    public ListOutboundPackagesQueryHandler(IOutboundPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }

    public async Task<RequestResult<IReadOnlyList<OutboundPackageDto>>> Handle(
        ListOutboundPackagesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.OutboundOrderId == Guid.Empty)
        {
            return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure(
                "outbound_packing.order.required",
                "Outbound order is required.");
        }

        var packages = await _packageRepository.ListByOrderIdAsync(request.OutboundOrderId, cancellationToken);
        var mapped = packages.Select(OutboundPackingMapping.Map).ToList();

        return RequestResult<IReadOnlyList<OutboundPackageDto>>.Success(mapped);
    }
}
