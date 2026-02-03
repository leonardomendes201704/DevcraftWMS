using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.AddAsnItem;

public sealed record AddAsnItemCommand(
    Guid AsnId,
    Guid ProductId,
    Guid UomId,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate) : IRequest<RequestResult<AsnItemDto>>;
