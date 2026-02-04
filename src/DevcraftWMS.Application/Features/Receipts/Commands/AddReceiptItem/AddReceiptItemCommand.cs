using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.AddReceiptItem;

public sealed record AddReceiptItemCommand(
    Guid ReceiptId,
    Guid ProductId,
    Guid? LotId,
    string? LotCode,
    DateOnly? ExpirationDate,
    Guid LocationId,
    Guid UomId,
    decimal Quantity,
    decimal? UnitCost,
    decimal? ActualWeightKg,
    decimal? ActualVolumeCm3) : IRequest<RequestResult<ReceiptItemDto>>;
