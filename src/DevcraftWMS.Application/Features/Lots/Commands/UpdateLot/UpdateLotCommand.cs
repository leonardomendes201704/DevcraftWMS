using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Commands.UpdateLot;

public sealed record UpdateLotCommand(
    Guid Id,
    Guid ProductId,
    string Code,
    DateOnly? ManufactureDate,
    DateOnly? ExpirationDate,
    LotStatus Status) : IRequest<RequestResult<LotDto>>;
