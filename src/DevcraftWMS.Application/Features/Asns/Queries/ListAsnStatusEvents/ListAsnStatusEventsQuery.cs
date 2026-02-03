using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.ListAsnStatusEvents;

public sealed record ListAsnStatusEventsQuery(Guid AsnId) : IRequest<RequestResult<IReadOnlyList<AsnStatusEventDto>>>;
