using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.Customers;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.DemoMvc.ViewComponents;

public sealed class CustomerSelectorViewComponent : ViewComponent
{
    private readonly CustomersApiClient _client;

    public CustomerSelectorViewComponent(CustomersApiClient client)
    {
        _client = client;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var token = HttpContext.Session.GetStringValue(SessionKeys.JwtToken);
        var returnUrl = $"{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

        if (string.IsNullOrWhiteSpace(token))
        {
            return View(new CustomerSelectorViewModel
            {
                IsAuthenticated = false,
                ReturnUrl = returnUrl
            });
        }

        Guid? selectedCustomerId = null;
        var customerIdRaw = HttpContext.Session.GetStringValue(SessionKeys.CustomerId);
        if (!string.IsNullOrWhiteSpace(customerIdRaw) && Guid.TryParse(customerIdRaw, out var parsed))
        {
            selectedCustomerId = parsed;
        }

        var query = new CustomerListQuery
        {
            PageNumber = 1,
            PageSize = 100,
            OrderBy = "Name",
            OrderDir = "asc",
            IncludeInactive = false
        };

        var result = await _client.ListAsync(query, HttpContext.RequestAborted);
        var customers = result.IsSuccess && result.Data is not null
            ? result.Data.Items
            : Array.Empty<CustomerDto>();

        return View(new CustomerSelectorViewModel
        {
            IsAuthenticated = true,
            ReturnUrl = returnUrl,
            Customers = customers,
            SelectedCustomerId = selectedCustomerId,
            ErrorMessage = result.IsSuccess ? null : (result.Error ?? "Unable to load customers.")
        });
    }
}
