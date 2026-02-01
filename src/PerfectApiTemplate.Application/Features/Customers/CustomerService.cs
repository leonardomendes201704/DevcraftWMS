using PerfectApiTemplate.Application.Abstractions;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Application.Features.Customers;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CustomerService(ICustomerRepository customerRepository, IDateTimeProvider dateTimeProvider)
    {
        _customerRepository = customerRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<CustomerDto>> CreateCustomerAsync(string name, string email, DateOnly dateOfBirth, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var emailExists = await _customerRepository.EmailExistsAsync(normalizedEmail, cancellationToken);
        if (emailExists)
        {
            return RequestResult<CustomerDto>.Failure("customer.email_exists", "A customer with this email already exists.");
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = normalizedEmail,
            DateOfBirth = dateOfBirth,
        };

        await _customerRepository.AddAsync(customer, cancellationToken);
        return RequestResult<CustomerDto>.Success(new CustomerDto(customer.Id, customer.Name, customer.Email, customer.DateOfBirth, customer.CreatedAtUtc));
    }

    public async Task<RequestResult<CustomerDto>> UpdateCustomerAsync(Guid id, string name, string email, DateOnly dateOfBirth, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return RequestResult<CustomerDto>.Failure("customer.not_found", "Customer not found.");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (!string.Equals(customer.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _customerRepository.EmailExistsAsync(normalizedEmail, id, cancellationToken);
            if (exists)
            {
                return RequestResult<CustomerDto>.Failure("customer.email_exists", "A customer with this email already exists.");
            }
        }

        customer.Name = name.Trim();
        customer.Email = normalizedEmail;
        customer.DateOfBirth = dateOfBirth;

        await _customerRepository.UpdateAsync(customer, cancellationToken);
        return RequestResult<CustomerDto>.Success(new CustomerDto(customer.Id, customer.Name, customer.Email, customer.DateOfBirth, customer.CreatedAtUtc));
    }

    public async Task<RequestResult<CustomerDto>> DeactivateCustomerAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return RequestResult<CustomerDto>.Failure("customer.not_found", "Customer not found.");
        }

        if (!customer.IsActive)
        {
            return RequestResult<CustomerDto>.Success(new CustomerDto(customer.Id, customer.Name, customer.Email, customer.DateOfBirth, customer.CreatedAtUtc));
        }

        customer.IsActive = false;
        await _customerRepository.UpdateAsync(customer, cancellationToken);
        return RequestResult<CustomerDto>.Success(new CustomerDto(customer.Id, customer.Name, customer.Email, customer.DateOfBirth, customer.CreatedAtUtc));
    }
}

