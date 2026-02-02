using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Commands.DeactivateLot;

public sealed record DeactivateLotCommand(Guid Id) : IRequest<RequestResult<LotDto>>;
