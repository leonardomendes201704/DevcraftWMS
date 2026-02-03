using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.ListAsnStatusEvents;

public sealed class ListAsnStatusEventsQueryHandler : IRequestHandler<ListAsnStatusEventsQuery, RequestResult<IReadOnlyList<AsnStatusEventDto>>>
{
    private readonly IAsnService _service;

    public ListAsnStatusEventsQueryHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<AsnStatusEventDto>>> Handle(ListAsnStatusEventsQuery request, CancellationToken cancellationToken)
        => _service.ListStatusEventsAsync(request.AsnId, cancellationToken);
}
