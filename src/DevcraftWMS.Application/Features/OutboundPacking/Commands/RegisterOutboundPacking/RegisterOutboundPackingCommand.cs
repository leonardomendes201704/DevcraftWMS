using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundPacking.Commands.RegisterOutboundPacking;

public sealed record RegisterOutboundPackingCommand(
    Guid OutboundOrderId,
    IReadOnlyList<OutboundPackageInput> Packages)
    : IRequest<RequestResult<IReadOnlyList<OutboundPackageDto>>>;
