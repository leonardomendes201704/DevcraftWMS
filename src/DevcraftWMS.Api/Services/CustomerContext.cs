using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Api.Middleware;

namespace DevcraftWMS.Api.Services;

public sealed class CustomerContext : ICustomerContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomerContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? CustomerId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null)
            {
                return null;
            }

            if (context.Items.TryGetValue(CustomerContextMiddleware.CustomerIdKey, out var value) && value is Guid customerId)
            {
                return customerId;
            }

            return null;
        }
    }
}
