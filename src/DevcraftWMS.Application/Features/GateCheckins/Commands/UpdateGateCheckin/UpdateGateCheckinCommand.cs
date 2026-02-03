using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.UpdateGateCheckin;

public sealed record UpdateGateCheckinCommand(
    Guid Id,
    Guid? InboundOrderId,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime ArrivalAtUtc,
    GateCheckinStatus Status,
    string? Notes) : IRequest<RequestResult<GateCheckinDetailDto>>;
