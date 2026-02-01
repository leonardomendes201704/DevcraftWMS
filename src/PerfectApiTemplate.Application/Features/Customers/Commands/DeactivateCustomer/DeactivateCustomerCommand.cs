using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Customers.Commands.DeactivateCustomer;

public sealed record DeactivateCustomerCommand(Guid Id) : IRequest<RequestResult<CustomerDto>>;

