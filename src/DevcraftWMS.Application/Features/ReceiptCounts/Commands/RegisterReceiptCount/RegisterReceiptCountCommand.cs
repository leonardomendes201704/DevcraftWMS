using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptCounts.Commands.RegisterReceiptCount;

public sealed record RegisterReceiptCountCommand(
    Guid ReceiptId,
    Guid InboundOrderItemId,
    decimal CountedQuantity,
    ReceiptCountMode Mode,
    string? Notes) : IRequest<RequestResult<ReceiptCountDto>>;
