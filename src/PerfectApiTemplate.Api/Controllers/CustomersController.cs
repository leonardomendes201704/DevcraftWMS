using MediatR;
using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.Api.Contracts;
using PerfectApiTemplate.Api.Extensions;
using PerfectApiTemplate.Application.Features.Customers.Commands;
using PerfectApiTemplate.Application.Features.Customers.Commands.DeactivateCustomer;
using PerfectApiTemplate.Application.Features.Customers.Commands.UpdateCustomer;
using PerfectApiTemplate.Application.Features.Customers.Queries;
using PerfectApiTemplate.Application.Features.Customers.Queries.ListCustomersPaged;

namespace PerfectApiTemplate.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCustomerCommand(request.Name, request.Email, request.DateOfBirth), cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetCustomerById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomerById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCustomerByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> ListCustomers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? cursor = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? search = null,
        [FromQuery] string? email = null,
        [FromQuery] string? name = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListCustomersQuery(pageNumber, pageSize, orderBy, orderDir, cursor, includeInactive, search, email, name), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> ListCustomersPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? search = null,
        [FromQuery] string? email = null,
        [FromQuery] string? name = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListCustomersPagedQuery(pageNumber, pageSize, orderBy, orderDir, search, email, name, includeInactive),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCustomerCommand(id, request.Name, request.Email, request.DateOfBirth), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateCustomerCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}

