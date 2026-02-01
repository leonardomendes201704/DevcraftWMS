using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Customers.Queries;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<RequestResult<CustomerDto>>;


