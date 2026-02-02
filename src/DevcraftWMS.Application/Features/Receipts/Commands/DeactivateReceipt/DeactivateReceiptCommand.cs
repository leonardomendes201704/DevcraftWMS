using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.DeactivateReceipt;

public sealed record DeactivateReceiptCommand(Guid Id) : IRequest<RequestResult<ReceiptDetailDto>>;
