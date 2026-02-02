using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.Products.Queries.ListProductsPaged;

public sealed record ListProductsPagedQuery(
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Code,
    string? Name,
    string? Category,
    string? Brand,
    string? Ean,
    bool? IsActive,
    bool IncludeInactive) : IRequest<RequestResult<PagedResult<ProductListItemDto>>>;
