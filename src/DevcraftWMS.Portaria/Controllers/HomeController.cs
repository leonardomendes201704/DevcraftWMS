using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Portaria.ApiClients;
using DevcraftWMS.Portaria.ViewModels.Home;

namespace DevcraftWMS.Portaria.Controllers;

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
