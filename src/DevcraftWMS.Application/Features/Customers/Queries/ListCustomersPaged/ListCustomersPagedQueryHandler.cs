using MediatR;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Customers.Queries.ListCustomersPaged;

public sealed class ListCustomersPagedQueryHandler : IRequestHandler<ListCustomersPagedQuery, RequestResult<PagedResult<CustomerDto>>>
{
    private readonly ICustomerRepository _customerRepository;

    public ListCustomersPagedQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<RequestResult<PagedResult<CustomerDto>>> Handle(ListCustomersPagedQuery request, CancellationToken cancellationToken)
    {
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "CreatedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        var total = await _customerRepository.CountAsync(request.Search, request.Email, request.Name, request.IncludeInactive, cancellationToken);
        var items = await _customerRepository.ListAsync(
            request.PageNumber,
            request.PageSize,
            orderBy,
            orderDir,
            request.Search,
            request.Email,
            request.Name,
            request.IncludeInactive,
            cancellationToken);

        var dtos = items.Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.DateOfBirth, c.CreatedAtUtc)).ToList();
        var result = new PagedResult<CustomerDto>(dtos, total, request.PageNumber, request.PageSize, orderBy, orderDir);
        return RequestResult<PagedResult<CustomerDto>>.Success(result);
    }
}


