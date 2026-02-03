using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.ListAsnItems;

public sealed record ListAsnItemsQuery(Guid AsnId) : IRequest<RequestResult<IReadOnlyList<AsnItemDto>>>;
