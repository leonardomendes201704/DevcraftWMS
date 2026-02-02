using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Queries.ListReceiptItemsPaged;

public sealed record ListReceiptItemsPagedQuery(
    Guid ReceiptId,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? ProductId = null,
    Guid? LocationId = null,
    Guid? LotId = null,
    bool? IsActive = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<ReceiptItemDto>>>;
