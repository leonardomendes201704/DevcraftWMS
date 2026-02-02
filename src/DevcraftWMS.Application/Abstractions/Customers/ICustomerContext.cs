namespace DevcraftWMS.Application.Abstractions.Customers;

public interface ICustomerContext
{
    Guid? CustomerId { get; }
}
