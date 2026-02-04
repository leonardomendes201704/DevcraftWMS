using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.UnitLoads.Commands.RelabelUnitLoadLabel;

public sealed record RelabelUnitLoadLabelCommand(
    Guid Id,
    string Reason,
    string? Notes) : IRequest<RequestResult<UnitLoadLabelDto>>;
