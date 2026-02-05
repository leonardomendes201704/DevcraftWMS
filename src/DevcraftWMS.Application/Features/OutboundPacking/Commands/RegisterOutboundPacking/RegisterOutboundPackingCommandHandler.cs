using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundPacking.Commands.RegisterOutboundPacking;

public sealed class RegisterOutboundPackingCommandHandler
    : IRequestHandler<RegisterOutboundPackingCommand, RequestResult<IReadOnlyList<OutboundPackageDto>>>
{
    private readonly IOutboundPackingService _service;

    public RegisterOutboundPackingCommandHandler(IOutboundPackingService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<OutboundPackageDto>>> Handle(RegisterOutboundPackingCommand request, CancellationToken cancellationToken)
        => _service.RegisterAsync(request.OutboundOrderId, request.Packages, cancellationToken);
}
