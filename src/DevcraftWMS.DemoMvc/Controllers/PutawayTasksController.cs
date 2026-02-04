using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.PutawayTasks;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class PutawayTasksController : Controller
{
    private readonly PutawayTasksApiClient _client;

    public PutawayTasksController(PutawayTasksApiClient client)
    {
        _client = client;
    }

    public async Task<IActionResult> Index([FromQuery] PutawayTaskQuery query, CancellationToken cancellationToken)
    {
        var result = await _client.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load putaway tasks.";
            return View(new PutawayTaskListPageViewModel
            {
                Items = Array.Empty<PutawayTaskListItemViewModel>(),
                Query = query
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "PutawayTasks",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["ReceiptId"] = query.ReceiptId?.ToString(),
                ["UnitLoadId"] = query.UnitLoadId?.ToString(),
                ["Status"] = query.Status?.ToString(),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new PutawayTaskListPageViewModel
        {
            Items = result.Data.Items,
            Query = query,
            Pagination = pagination
        });
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Putaway task not found.";
            return RedirectToAction("Index");
        }

        var suggestions = await _client.GetSuggestionsAsync(id, 5, cancellationToken);
        var suggestionItems = suggestions.IsSuccess && suggestions.Data is not null
            ? suggestions.Data
            : Array.Empty<PutawaySuggestionViewModel>();

        var locationOptions = suggestionItems
            .Select(s => new SelectListItem
            {
                Value = s.LocationId.ToString(),
                Text = $"{s.LocationCode} - {s.ZoneName}"
            })
            .ToList();
        locationOptions.Insert(0, new SelectListItem { Value = string.Empty, Text = "Select..." });

        return View(new PutawayTaskDetailsPageViewModel
        {
            Task = result.Data,
            Suggestions = suggestionItems,
            Confirm = new PutawayTaskConfirmViewModel
            {
                LocationOptions = locationOptions
            },
            Reassign = new PutawayTaskReassignViewModel()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(Guid id, PutawayTaskConfirmViewModel model, CancellationToken cancellationToken)
    {
        if (model.LocationId == Guid.Empty)
        {
            TempData["Error"] = "Please select a destination location.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _client.ConfirmAsync(id, model.LocationId, model.Notes, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to confirm putaway.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Putaway confirmed successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reassign(Guid id, PutawayTaskReassignViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.AssigneeEmail))
        {
            TempData["Error"] = "Please provide an assignee email.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (string.IsNullOrWhiteSpace(model.Reason))
        {
            TempData["Error"] = "Please provide a reassignment reason.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _client.ReassignAsync(id, model.AssigneeEmail.Trim(), model.Reason.Trim(), cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to reassign putaway task.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Putaway task reassigned successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
