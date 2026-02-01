using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Customers;

namespace DevcraftWMS.Application.Features.Customers.Commands;

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


