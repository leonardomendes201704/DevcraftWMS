using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundPacking.Queries.ListOutboundPackages;

public sealed record ListOutboundPackagesQuery(Guid OutboundOrderId)
    : IRequest<RequestResult<IReadOnlyList<OutboundPackageDto>>>;
