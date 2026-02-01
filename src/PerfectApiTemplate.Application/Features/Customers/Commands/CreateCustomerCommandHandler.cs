using MediatR;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Features.Customers;

namespace PerfectApiTemplate.Application.Features.Customers.Commands;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, RequestResult<CustomerDto>>
{
    private readonly ICustomerService _customerService;

    public CreateCustomerCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<RequestResult<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        return await _customerService.CreateCustomerAsync(request.Name, request.Email, request.DateOfBirth, cancellationToken);
    }
}

