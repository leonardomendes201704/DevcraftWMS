using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundChecks.Commands.RegisterOutboundCheck;

public sealed class RegisterOutboundCheckCommandHandler
    : IRequestHandler<RegisterOutboundCheckCommand, RequestResult<OutboundCheckDto>>
{
    private readonly IOutboundCheckService _service;

    public RegisterOutboundCheckCommandHandler(IOutboundCheckService service)
    {
        _service = service;
    }

    public Task<RequestResult<OutboundCheckDto>> Handle(RegisterOutboundCheckCommand request, CancellationToken cancellationToken)
        => _service.RegisterAsync(request.OutboundOrderId, request.Items, request.Notes, cancellationToken);
}
