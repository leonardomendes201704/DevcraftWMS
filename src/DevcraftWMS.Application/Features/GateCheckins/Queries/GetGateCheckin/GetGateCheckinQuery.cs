using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.GateCheckins.Queries.GetGateCheckin;

public sealed record GetGateCheckinQuery(Guid Id) : IRequest<RequestResult<GateCheckinDetailDto>>;
