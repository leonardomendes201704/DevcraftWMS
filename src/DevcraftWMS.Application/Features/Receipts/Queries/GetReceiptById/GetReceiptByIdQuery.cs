using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Queries.GetReceiptById;

public sealed record GetReceiptByIdQuery(Guid Id) : IRequest<RequestResult<ReceiptDetailDto>>;
