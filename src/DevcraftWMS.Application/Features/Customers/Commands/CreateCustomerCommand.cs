using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Customers.Commands;

public sealed record CreateCustomerCommand(string Name, string Email, DateOnly DateOfBirth) : IRequest<RequestResult<CustomerDto>>;


