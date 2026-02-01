using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Customers.Commands.DeactivateCustomer;

public sealed class DeactivateCustomerCommandHandler : IRequestHandler<DeactivateCustomerCommand, RequestResult<CustomerDto>>
{
    private readonly ICustomerService _service;

    public DeactivateCustomerCommandHandler(ICustomerService service)
    {
        _service = service;
    }

    public Task<RequestResult<CustomerDto>> Handle(DeactivateCustomerCommand request, CancellationToken cancellationToken)
        => _service.DeactivateCustomerAsync(request.Id, cancellationToken);
}


