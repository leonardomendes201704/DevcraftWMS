using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.ListAsnsPaged;

public sealed record ListAsnsPagedQuery(
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    Guid? WarehouseId,
    string? AsnNumber,
    string? SupplierName,
    string? DocumentNumber,
    AsnStatus? Status,
    DateOnly? ExpectedFrom,
    DateOnly? ExpectedTo,
    bool? IsActive,
    bool IncludeInactive) : IRequest<RequestResult<PagedResult<AsnListItemDto>>>;
