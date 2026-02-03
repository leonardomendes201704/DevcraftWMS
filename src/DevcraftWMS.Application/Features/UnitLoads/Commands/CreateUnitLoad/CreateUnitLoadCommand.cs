using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.UnitLoads.Commands.CreateUnitLoad;

public sealed record CreateUnitLoadCommand(
    Guid ReceiptId,
    string? SsccExternal,
    string? Notes) : IRequest<RequestResult<UnitLoadDetailDto>>;
