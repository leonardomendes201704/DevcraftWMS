using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.UnitLoads.Commands.PrintUnitLoadLabel;

public sealed class PrintUnitLoadLabelCommandHandler : IRequestHandler<PrintUnitLoadLabelCommand, RequestResult<UnitLoadLabelDto>>
{
    private readonly IUnitLoadService _service;

    public PrintUnitLoadLabelCommandHandler(IUnitLoadService service)
    {
        _service = service;
    }

    public Task<RequestResult<UnitLoadLabelDto>> Handle(PrintUnitLoadLabelCommand request, CancellationToken cancellationToken)
        => _service.PrintLabelAsync(request.Id, cancellationToken);
}
