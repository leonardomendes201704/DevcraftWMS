using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.DemoMvc.Infrastructure;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class CustomerContextController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetCustomer(string? customerId, string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            HttpContext.Session.Remove(SessionKeys.CustomerId);
            TempData["Warning"] = "Customer context cleared.";
            return Redirect(returnUrl ?? "/");
        }

        if (!Guid.TryParse(customerId, out _))
        {
            TempData["Error"] = "Invalid customer selection.";
            return Redirect(returnUrl ?? "/");
        }

        HttpContext.Session.SetStringValue(SessionKeys.CustomerId, customerId);
        TempData["Success"] = "Customer context updated.";
        return Redirect(returnUrl ?? "/");
    }
}
