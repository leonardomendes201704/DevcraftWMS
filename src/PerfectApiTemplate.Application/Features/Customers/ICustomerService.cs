using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Customers;

public interface ICustomerService
{
    Task<RequestResult<CustomerDto>> CreateCustomerAsync(string name, string email, DateOnly dateOfBirth, CancellationToken cancellationToken);
    Task<RequestResult<CustomerDto>> UpdateCustomerAsync(Guid id, string name, string email, DateOnly dateOfBirth, CancellationToken cancellationToken);
    Task<RequestResult<CustomerDto>> DeactivateCustomerAsync(Guid id, CancellationToken cancellationToken);
}

