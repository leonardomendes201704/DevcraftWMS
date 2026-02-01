using MediatR;
using PerfectApiTemplate.Application.Abstractions;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Customers.Queries;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, RequestResult<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<RequestResult<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return RequestResult<CustomerDto>.Failure("customer.not_found", "Customer not found.");
        }

        var dto = new CustomerDto(customer.Id, customer.Name, customer.Email, customer.DateOfBirth, customer.CreatedAtUtc);
        return RequestResult<CustomerDto>.Success(dto);
    }
}

