using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.UnitLoads.Commands.PrintUnitLoadLabel;

public sealed record PrintUnitLoadLabelCommand(Guid Id) : IRequest<RequestResult<UnitLoadLabelDto>>;
