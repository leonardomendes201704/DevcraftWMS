using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.DemoMvc.ApiClients;
using PerfectApiTemplate.DemoMvc.Infrastructure;
using PerfectApiTemplate.DemoMvc.ViewModels.Auth;

namespace PerfectApiTemplate.DemoMvc.Controllers;

public sealed class AuthController : Controller
{
    private readonly AuthApiClient _authClient;

    public AuthController(AuthApiClient authClient)
    {
        _authClient = authClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? mode, CancellationToken cancellationToken)
    {
        if (string.Equals(mode, "token", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(model.Token))
            {
                ModelState.AddModelError(nameof(model.Token), "Token is required.");
                return View(model);
            }

            HttpContext.Session.SetStringValue(SessionKeys.JwtToken, model.Token.Trim());
            TempData["Success"] = "Token stored successfully.";
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(string.Empty, "Email and password are required.");
            return View(model);
        }

        var result = await _authClient.LoginAsync(model.Email, model.Password, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Login failed.");
            return View(model);
        }

        HttpContext.Session.SetStringValue(SessionKeys.JwtToken, result.Data.Token);
        TempData["Success"] = "Logged in successfully.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove(SessionKeys.JwtToken);
        TempData["Success"] = "Logged out.";
        return RedirectToAction("Login", "Auth");
    }
}
