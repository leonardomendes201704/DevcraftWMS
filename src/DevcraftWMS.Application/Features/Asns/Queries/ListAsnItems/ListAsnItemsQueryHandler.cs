using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.ListAsnItems;

public sealed class ListAsnItemsQueryHandler : IRequestHandler<ListAsnItemsQuery, RequestResult<IReadOnlyList<AsnItemDto>>>
{
    private readonly IAsnService _service;

    public ListAsnItemsQueryHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<AsnItemDto>>> Handle(ListAsnItemsQuery request, CancellationToken cancellationToken)
        => _service.ListItemsAsync(request.AsnId, cancellationToken);
}
