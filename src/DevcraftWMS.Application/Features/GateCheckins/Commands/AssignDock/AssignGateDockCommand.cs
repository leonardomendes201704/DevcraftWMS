using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.AssignDock;

public sealed record AssignGateDockCommand(Guid Id, string DockCode) : IRequest<RequestResult<GateCheckinDetailDto>>;
