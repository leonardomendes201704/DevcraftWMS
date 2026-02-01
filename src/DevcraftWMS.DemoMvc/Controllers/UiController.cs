using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class UiController : Controller
{
    [HttpGet]
    public IActionResult Showcase()
    {
        return View();
    }
}


