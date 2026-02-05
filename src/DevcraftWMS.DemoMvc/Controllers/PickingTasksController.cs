using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.PickingTasks;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class PickingTasksController : Controller
{
    private readonly PickingTasksApiClient _client;

    public PickingTasksController(PickingTasksApiClient client)
    {
        _client = client;
    }

    public async Task<IActionResult> Index([FromQuery] PickingTaskQuery query, CancellationToken cancellationToken)
    {
        var result = await _client.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load picking tasks.";
            return View(new PickingTaskListPageViewModel
            {
                Items = Array.Empty<PickingTaskListItemViewModel>(),
                Query = query
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "PickingTasks",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["OutboundOrderId"] = query.OutboundOrderId?.ToString(),
                ["AssignedUserId"] = query.AssignedUserId?.ToString(),
                ["Status"] = query.Status?.ToString(),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new PickingTaskListPageViewModel
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
            TempData["Error"] = result.Error ?? "Picking task not found.";
            return RedirectToAction("Index");
        }

        var statusOptions = Enum.GetValues<DevcraftWMS.DemoMvc.Enums.PickingTaskStatus>()
            .Select(status => new SelectListItem
            {
                Value = status.ToString(),
                Text = status.GetDisplayName(),
                Selected = status == result.Data.Status
            })
            .ToList();

        return View(new PickingTaskDetailsPageViewModel
        {
            Task = result.Data,
            Confirm = new PickingTaskConfirmViewModel
            {
                Items = result.Data.Items
                    .Select(item => new PickingTaskConfirmItemViewModel
                    {
                        PickingTaskItemId = item.Id,
                        QuantityPicked = item.QuantityPicked
                    })
                    .ToList()
            },
            StatusOptions = statusOptions
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(Guid id, PickingTaskConfirmViewModel model, CancellationToken cancellationToken)
    {
        if (model.Items.Count == 0)
        {
            TempData["Error"] = "Please provide at least one picked item.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (model.Items.Any(i => i.QuantityPicked < 0))
        {
            TempData["Error"] = "Picked quantity cannot be negative.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _client.ConfirmAsync(id, model.Items, model.Notes, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to confirm picking.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Picking confirmed successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
