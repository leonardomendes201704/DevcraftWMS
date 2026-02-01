using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Customers.Queries;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<RequestResult<CustomerDto>>;

