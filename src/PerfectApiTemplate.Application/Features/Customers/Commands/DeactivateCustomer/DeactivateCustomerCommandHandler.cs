using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Customers.Commands.DeactivateCustomer;

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

