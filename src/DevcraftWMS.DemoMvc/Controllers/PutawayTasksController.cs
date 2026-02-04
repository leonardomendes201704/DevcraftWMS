using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.PutawayTasks;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

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

        return View(new PutawayTaskDetailsPageViewModel
        {
            Task = result.Data
        });
    }
}
