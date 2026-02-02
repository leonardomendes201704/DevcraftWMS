using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Commands.DeactivateLot;

public sealed class DeactivateLotCommandHandler : IRequestHandler<DeactivateLotCommand, RequestResult<LotDto>>
{
    private readonly ILotService _service;

    public DeactivateLotCommandHandler(ILotService service)
    {
        _service = service;
    }

    public Task<RequestResult<LotDto>> Handle(DeactivateLotCommand request, CancellationToken cancellationToken)
        => _service.DeactivateLotAsync(request.Id, cancellationToken);
}
