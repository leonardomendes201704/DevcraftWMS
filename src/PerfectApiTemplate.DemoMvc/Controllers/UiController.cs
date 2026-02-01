using Microsoft.AspNetCore.Mvc;

namespace PerfectApiTemplate.DemoMvc.Controllers;

public sealed class UiController : Controller
{
    [HttpGet]
    public IActionResult Showcase()
    {
        return View();
    }
}

