using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.UnitLoads.Commands.CreateUnitLoad;

public sealed class CreateUnitLoadCommandHandler : IRequestHandler<CreateUnitLoadCommand, RequestResult<UnitLoadDetailDto>>
{
    private readonly IUnitLoadService _service;

    public CreateUnitLoadCommandHandler(IUnitLoadService service)
    {
        _service = service;
    }

    public Task<RequestResult<UnitLoadDetailDto>> Handle(CreateUnitLoadCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(request.ReceiptId, request.SsccExternal, request.Notes, cancellationToken);
}
