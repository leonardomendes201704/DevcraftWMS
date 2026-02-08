using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.DockSchedules;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class DockSchedulesController : Controller
{
    private readonly DockSchedulesApiClient _client;

    public DockSchedulesController(DockSchedulesApiClient client)
    {
        _client = client;
    }

    public async Task<IActionResult> Index([FromQuery] DockScheduleListQueryViewModel query, CancellationToken cancellationToken)
    {
        query = query.Normalize();
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load dock schedules.";
            return View(new DockScheduleListPageViewModel { Query = query });
        }

        var result = await _client.ListAsync(query, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to load dock schedules.";
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data?.PageNumber ?? query.PageNumber,
            PageSize = result.Data?.PageSize ?? query.PageSize,
            TotalCount = result.Data?.TotalCount ?? 0,
            Action = "Index",
            Controller = "DockSchedules",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["DockCode"] = query.DockCode,
                ["Status"] = query.Status?.ToString(),
                ["FromUtc"] = query.FromUtc?.ToString("o"),
                ["ToUtc"] = query.ToUtc?.ToString("o"),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new DockScheduleListPageViewModel
        {
            Query = query,
            Items = result.Data?.Items ?? Array.Empty<DockScheduleListItemViewModel>(),
            Pagination = pagination
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to view dock schedules.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Dock schedule not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new DockScheduleDetailsPageViewModel { Schedule = result.Data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reschedule(Guid id, RescheduleDockScheduleRequestViewModel model, CancellationToken cancellationToken)
    {
        var result = await _client.RescheduleAsync(id, model, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to reschedule dock slot.";
        }
        else
        {
            TempData["Success"] = "Dock schedule updated.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, CancelDockScheduleRequestViewModel model, CancellationToken cancellationToken)
    {
        var result = await _client.CancelAsync(id, model, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to cancel dock schedule.";
        }
        else
        {
            TempData["Success"] = "Dock schedule canceled.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(Guid id, AssignDockScheduleRequestViewModel model, CancellationToken cancellationToken)
    {
        var result = await _client.AssignAsync(id, model, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to assign dock schedule.";
        }
        else
        {
            TempData["Success"] = "Dock schedule assigned.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private bool HasCustomerContext()
        => !string.IsNullOrWhiteSpace(HttpContext.Session.GetStringValue(SessionKeys.CustomerId));
}
