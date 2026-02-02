using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Commands.CreateLot;

public sealed class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, RequestResult<LotDto>>
{
    private readonly ILotService _service;

    public CreateLotCommandHandler(ILotService service)
    {
        _service = service;
    }

    public Task<RequestResult<LotDto>> Handle(CreateLotCommand request, CancellationToken cancellationToken)
        => _service.CreateLotAsync(
            request.ProductId,
            request.Code,
            request.ManufactureDate,
            request.ExpirationDate,
            request.Status,
            cancellationToken);
}
