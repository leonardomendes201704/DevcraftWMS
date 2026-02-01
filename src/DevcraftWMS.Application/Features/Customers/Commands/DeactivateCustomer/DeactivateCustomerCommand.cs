using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Customers.Commands.DeactivateCustomer;

public sealed record DeactivateCustomerCommand(Guid Id) : IRequest<RequestResult<CustomerDto>>;


