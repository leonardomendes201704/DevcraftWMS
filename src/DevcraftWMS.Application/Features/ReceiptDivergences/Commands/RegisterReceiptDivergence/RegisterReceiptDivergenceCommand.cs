using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Commands.RegisterReceiptDivergence;

public sealed record RegisterReceiptDivergenceCommand(
    Guid ReceiptId,
    Guid? InboundOrderItemId,
    ReceiptDivergenceType Type,
    string? Notes,
    ReceiptDivergenceEvidenceInput? Evidence) : IRequest<RequestResult<ReceiptDivergenceDto>>;
