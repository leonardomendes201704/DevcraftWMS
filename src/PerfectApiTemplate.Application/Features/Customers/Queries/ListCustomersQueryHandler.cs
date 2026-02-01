using MediatR;
using PerfectApiTemplate.Application.Abstractions;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Customers.Queries;

public sealed class ListCustomersQueryHandler : IRequestHandler<ListCustomersQuery, RequestResult<CursorPaginationResult<CustomerDto>>>
{
    private const int MaxPageSize = 100;
    private readonly ICustomerRepository _customerRepository;

    public ListCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<RequestResult<CursorPaginationResult<CustomerDto>>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > MaxPageSize ? 20 : request.PageSize;
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "CreatedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        if (CursorToken.TryParse(request.Cursor, out var token) && orderBy.Equals("CreatedAtUtc", StringComparison.OrdinalIgnoreCase))
        {
            if (token.Parts.Count >= 2 && DateTime.TryParse(token.Parts[0], out var cursorTime) && Guid.TryParse(token.Parts[1], out var cursorId))
            {
                var items = await _customerRepository.ListByCreatedAtCursorAsync(pageSize, orderDir, cursorTime, cursorId, request.IncludeInactive, cancellationToken);
                var dtos = items.Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.DateOfBirth, c.CreatedAtUtc)).ToList();
                var nextCursor = dtos.Count == 0
                    ? null
                    : CursorToken.Build(dtos[^1].CreatedAtUtc.ToString("O"), dtos[^1].Id);
                return RequestResult<CursorPaginationResult<CustomerDto>>.Success(
                    new CursorPaginationResult<CustomerDto>(dtos, nextCursor, pageSize, orderBy, orderDir));
            }
        }

        var customers = await _customerRepository.ListAsync(
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            request.Search,
            request.Email,
            request.Name,
            request.IncludeInactive,
            cancellationToken);
        var fallback = customers.Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.DateOfBirth, c.CreatedAtUtc)).ToList();
        return RequestResult<CursorPaginationResult<CustomerDto>>.Success(
            new CursorPaginationResult<CustomerDto>(fallback, null, pageSize, orderBy, orderDir));
    }
}
