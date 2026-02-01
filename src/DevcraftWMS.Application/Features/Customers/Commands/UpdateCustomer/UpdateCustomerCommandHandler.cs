using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, RequestResult<CustomerDto>>
{
    private readonly ICustomerService _service;

    public UpdateCustomerCommandHandler(ICustomerService service)
    {
        _service = service;
    }

    public Task<RequestResult<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        => _service.UpdateCustomerAsync(request.Id, request.Name, request.Email, request.DateOfBirth, cancellationToken);
}


