using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.UpdateGateCheckin;

public sealed class UpdateGateCheckinCommandHandler : IRequestHandler<UpdateGateCheckinCommand, RequestResult<GateCheckinDetailDto>>
{
    private readonly IGateCheckinService _service;

    public UpdateGateCheckinCommandHandler(IGateCheckinService service)
    {
        _service = service;
    }

    public Task<RequestResult<GateCheckinDetailDto>> Handle(UpdateGateCheckinCommand request, CancellationToken cancellationToken)
        => _service.UpdateAsync(
            request.Id,
            request.InboundOrderId,
            request.DocumentNumber,
            request.VehiclePlate,
            request.DriverName,
            request.CarrierName,
            request.ArrivalAtUtc,
            request.Status,
            request.Notes,
            cancellationToken);
}
