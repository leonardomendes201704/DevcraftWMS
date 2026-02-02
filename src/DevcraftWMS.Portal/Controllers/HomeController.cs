using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Portal.ApiClients;
using DevcraftWMS.Portal.ViewModels.Home;

namespace DevcraftWMS.Portal.Controllers;

public sealed class HomeController : Controller
{
    private readonly HealthApiClient _healthClient;

    public HomeController(HealthApiClient healthClient)
    {
        _healthClient = healthClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await _healthClient.GetHealthAsync(cancellationToken);
        var model = new HomeViewModel
        {
            IsApiHealthy = result.IsSuccess,
            StatusMessage = result.IsSuccess ? "API online" : (result.Error ?? "API unavailable")
        };

        return View(model);
    }
}
