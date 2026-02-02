using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Commands.UpdateAisle;

public sealed record UpdateAisleCommand(Guid Id, Guid SectionId, string Code, string Name)
    : IRequest<RequestResult<AisleDto>>;
