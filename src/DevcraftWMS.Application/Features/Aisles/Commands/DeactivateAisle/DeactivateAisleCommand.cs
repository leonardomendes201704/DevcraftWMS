using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Commands.DeactivateAisle;

public sealed record DeactivateAisleCommand(Guid Id) : IRequest<RequestResult<AisleDto>>;
