using MediatR;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Customers.Queries.ListCustomersPaged;

public sealed record ListCustomersPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Search = null,
    string? Email = null,
    string? Name = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<CustomerDto>>>;

