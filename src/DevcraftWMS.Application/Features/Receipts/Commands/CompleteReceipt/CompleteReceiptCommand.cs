using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.CompleteReceipt;

public sealed record CompleteReceiptCommand(Guid Id) : IRequest<RequestResult<ReceiptDetailDto>>;
