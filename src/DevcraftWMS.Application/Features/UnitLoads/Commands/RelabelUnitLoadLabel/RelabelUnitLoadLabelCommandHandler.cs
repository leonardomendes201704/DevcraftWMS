using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.UnitLoads.Commands.RelabelUnitLoadLabel;

public sealed class RelabelUnitLoadLabelCommandHandler
    : IRequestHandler<RelabelUnitLoadLabelCommand, RequestResult<UnitLoadLabelDto>>
{
    private readonly IUnitLoadService _service;

    public RelabelUnitLoadLabelCommandHandler(IUnitLoadService service)
    {
        _service = service;
    }

    public Task<RequestResult<UnitLoadLabelDto>> Handle(RelabelUnitLoadLabelCommand request, CancellationToken cancellationToken)
        => _service.RelabelAsync(request.Id, request.Reason, request.Notes, cancellationToken);
}
