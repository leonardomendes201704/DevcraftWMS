using MediatR;
using PerfectApiTemplate.Application.Abstractions;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Customers.Queries.ListCustomersPaged;

public sealed class ListCustomersPagedQueryHandler : IRequestHandler<ListCustomersPagedQuery, RequestResult<PagedResult<CustomerDto>>>
{
    private const int MaxPageSize = 100;
    private readonly ICustomerRepository _customerRepository;

    public ListCustomersPagedQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<RequestResult<PagedResult<CustomerDto>>> Handle(ListCustomersPagedQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > MaxPageSize ? 20 : request.PageSize;
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "CreatedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        var total = await _customerRepository.CountAsync(request.Search, request.Email, request.Name, request.IncludeInactive, cancellationToken);
        var items = await _customerRepository.ListAsync(
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            request.Search,
            request.Email,
            request.Name,
            request.IncludeInactive,
            cancellationToken);

        var dtos = items.Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.DateOfBirth, c.CreatedAtUtc)).ToList();
        var result = new PagedResult<CustomerDto>(dtos, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<CustomerDto>>.Success(result);
    }
}

