using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.DemoMvc.ApiClients;
using PerfectApiTemplate.DemoMvc.Infrastructure;
using PerfectApiTemplate.DemoMvc.ViewModels.Email;

namespace PerfectApiTemplate.DemoMvc.Controllers;

public sealed class EmailController : Controller
{
    private readonly EmailApiClient _client;
    private readonly DemoOptions _options;

    public EmailController(EmailApiClient client, IOptions<DemoOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = new EmailTestViewModel
        {
            From = _options.Demo.AdminEmail,
            RecentLogs = GetLogs()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Send(EmailTestViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.RecentLogs = GetLogs();
            return View("Index", model);
        }

        var request = new EmailSendRequest(model.From, model.To, model.Subject, model.Body, model.IsHtml);
        var result = await _client.SendAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to send email.";
            model.RecentLogs = GetLogs();
            return View("Index", model);
        }

        AddLog(new EmailLogEntry(DateTime.UtcNow, model.To, model.Subject, "Sent", null));
        TempData["Success"] = "Email queued for delivery.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Preview(EmailTestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.RecentLogs = GetLogs();
            return View("Index", model);
        }

        model.PreviewHtml = model.IsHtml ? model.Body : $"<pre>{System.Net.WebUtility.HtmlEncode(model.Body)}</pre>";
        model.RecentLogs = GetLogs();
        return View("Index", model);
    }

    private List<EmailLogEntry> GetLogs()
    {
        return HttpContext.Session.GetJson<List<EmailLogEntry>>(SessionKeys.EmailLog) ?? new List<EmailLogEntry>();
    }

    private void AddLog(EmailLogEntry entry)
    {
        var logs = GetLogs();
        logs.Insert(0, entry);
        if (logs.Count > 10)
        {
            logs = logs.Take(10).ToList();
        }

        HttpContext.Session.SetJson(SessionKeys.EmailLog, logs);
    }
}

