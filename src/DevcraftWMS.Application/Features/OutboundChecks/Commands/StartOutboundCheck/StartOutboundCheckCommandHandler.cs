using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundChecks.Commands.StartOutboundCheck;

public sealed class StartOutboundCheckCommandHandler
    : IRequestHandler<StartOutboundCheckCommand, RequestResult<OutboundCheckDto>>
{
    private readonly IOutboundCheckService _service;

    public StartOutboundCheckCommandHandler(IOutboundCheckService service)
    {
        _service = service;
    }

    public Task<RequestResult<OutboundCheckDto>> Handle(
        StartOutboundCheckCommand request,
        CancellationToken cancellationToken)
        => _service.StartAsync(request.Id, cancellationToken);
}
