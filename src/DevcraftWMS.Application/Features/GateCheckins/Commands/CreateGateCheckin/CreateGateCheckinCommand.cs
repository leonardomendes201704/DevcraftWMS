using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.CreateGateCheckin;

public sealed record CreateGateCheckinCommand(
    Guid? InboundOrderId,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime? ArrivalAtUtc,
    string? Notes) : IRequest<RequestResult<GateCheckinDetailDto>>;
