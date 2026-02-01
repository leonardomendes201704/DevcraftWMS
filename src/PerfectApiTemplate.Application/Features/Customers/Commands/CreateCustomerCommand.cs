using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Customers.Commands;

public sealed record CreateCustomerCommand(string Name, string Email, DateOnly DateOfBirth) : IRequest<RequestResult<CustomerDto>>;

