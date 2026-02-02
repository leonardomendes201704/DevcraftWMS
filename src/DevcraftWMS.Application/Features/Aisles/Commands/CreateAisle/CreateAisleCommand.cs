using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Commands.CreateAisle;

public sealed record CreateAisleCommand(Guid SectionId, string Code, string Name)
    : IRequest<RequestResult<AisleDto>>;
