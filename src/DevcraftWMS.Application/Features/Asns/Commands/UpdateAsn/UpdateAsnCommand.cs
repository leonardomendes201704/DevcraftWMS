using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;

namespace DevcraftWMS.Application.Features.Asns.Commands.UpdateAsn;

public sealed record UpdateAsnCommand(
    Guid Id,
    Guid WarehouseId,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    DateOnly? ExpectedArrivalDate,
    string? Notes) : IRequest<RequestResult<AsnDetailDto>>;
