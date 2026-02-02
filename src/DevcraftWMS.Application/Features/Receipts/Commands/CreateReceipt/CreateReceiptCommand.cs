using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.CreateReceipt;

public sealed record CreateReceiptCommand(
    Guid WarehouseId,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    string? Notes) : IRequest<RequestResult<ReceiptDetailDto>>;
