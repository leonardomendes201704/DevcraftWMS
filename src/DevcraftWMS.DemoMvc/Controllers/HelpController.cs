using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class HelpController : Controller
{
    [HttpGet("/help/manual")]
    public IActionResult Manual()
    {
        ViewData["Title"] = "Manual do Sistema";
        return View();
    }
}