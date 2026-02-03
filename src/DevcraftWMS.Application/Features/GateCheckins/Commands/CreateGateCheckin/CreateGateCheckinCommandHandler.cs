using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.CreateGateCheckin;

public sealed class CreateGateCheckinCommandHandler : IRequestHandler<CreateGateCheckinCommand, RequestResult<GateCheckinDetailDto>>
{
    private readonly IGateCheckinService _service;

    public CreateGateCheckinCommandHandler(IGateCheckinService service)
    {
        _service = service;
    }

    public Task<RequestResult<GateCheckinDetailDto>> Handle(CreateGateCheckinCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(
            request.InboundOrderId,
            request.DocumentNumber,
            request.VehiclePlate,
            request.DriverName,
            request.CarrierName,
            request.ArrivalAtUtc,
            request.Notes,
            cancellationToken);
}
