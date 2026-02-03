using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.StartReceiptFromInboundOrder;

public sealed record StartReceiptFromInboundOrderCommand(Guid InboundOrderId) : IRequest<RequestResult<ReceiptDetailDto>>;
