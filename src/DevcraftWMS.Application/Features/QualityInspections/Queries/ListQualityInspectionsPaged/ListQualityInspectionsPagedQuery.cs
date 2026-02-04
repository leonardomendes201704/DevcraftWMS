using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Queries.ListQualityInspectionsPaged;

public sealed record ListQualityInspectionsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    QualityInspectionStatus? Status = null,
    Guid? WarehouseId = null,
    Guid? ReceiptId = null,
    Guid? ProductId = null,
    Guid? LotId = null,
    DateTime? CreatedFromUtc = null,
    DateTime? CreatedToUtc = null,
    bool? IsActive = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<QualityInspectionListItemDto>>>;
