using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Customers.Queries.ListCustomersPaged;

public sealed record ListCustomersPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Search = null,
    string? Email = null,
    string? Name = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<CustomerDto>>>;


