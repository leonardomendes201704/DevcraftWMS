using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Commands.UpdateLot;

public sealed class UpdateLotCommandHandler : IRequestHandler<UpdateLotCommand, RequestResult<LotDto>>
{
    private readonly ILotService _service;

    public UpdateLotCommandHandler(ILotService service)
    {
        _service = service;
    }

    public Task<RequestResult<LotDto>> Handle(UpdateLotCommand request, CancellationToken cancellationToken)
        => _service.UpdateLotAsync(
            request.Id,
            request.ProductId,
            request.Code,
            request.ManufactureDate,
            request.ExpirationDate,
            request.Status,
            cancellationToken);
}
