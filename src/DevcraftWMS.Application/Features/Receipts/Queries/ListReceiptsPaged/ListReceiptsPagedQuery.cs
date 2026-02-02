using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Application.Features.Receipts;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Queries.ListReceiptsPaged;

public sealed record ListReceiptsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? WarehouseId = null,
    string? ReceiptNumber = null,
    string? DocumentNumber = null,
    string? SupplierName = null,
    ReceiptStatus? Status = null,
    DateOnly? ReceivedFrom = null,
    DateOnly? ReceivedTo = null,
    bool? IsActive = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<ReceiptListItemDto>>>;
