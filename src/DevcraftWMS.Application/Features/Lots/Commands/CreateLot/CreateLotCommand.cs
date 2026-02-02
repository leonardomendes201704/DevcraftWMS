using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Commands.CreateLot;

public sealed record CreateLotCommand(
    Guid ProductId,
    string Code,
    DateOnly? ManufactureDate,
    DateOnly? ExpirationDate,
    LotStatus Status) : IRequest<RequestResult<LotDto>>;
