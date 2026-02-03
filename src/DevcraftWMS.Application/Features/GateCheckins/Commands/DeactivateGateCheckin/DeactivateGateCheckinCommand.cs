using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.DeactivateGateCheckin;

public sealed record DeactivateGateCheckinCommand(Guid Id) : IRequest<RequestResult<GateCheckinDetailDto>>;
