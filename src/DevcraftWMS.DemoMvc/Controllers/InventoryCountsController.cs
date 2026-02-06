using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.InventoryCounts;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class InventoryCountsController : Controller
{
    private readonly InventoryCountsApiClient _countsClient;

    public InventoryCountsController(InventoryCountsApiClient countsClient)
    {
        _countsClient = countsClient;
    }

    public async Task<IActionResult> Index([FromQuery] InventoryCountListQueryViewModel query, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load inventory counts.";
            return View(new InventoryCountListPageViewModel { Query = query });
        }

        var result = await _countsClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to load inventory counts.";
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data?.PageNumber ?? query.PageNumber,
            PageSize = result.Data?.PageSize ?? query.PageSize,
            TotalCount = result.Data?.TotalCount ?? 0,
            Action = "Index",
            Controller = "InventoryCounts",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["LocationId"] = query.LocationId?.ToString(),
                ["Status"] = query.Status?.ToString(),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new InventoryCountListPageViewModel
        {
            Query = query,
            Items = result.Data?.Items ?? Array.Empty<InventoryCountListItemViewModel>(),
            Pagination = pagination
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to view inventory counts.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _countsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Inventory count not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new InventoryCountDetailsPageViewModel { Count = result.Data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(Guid id, CancellationToken cancellationToken)
    {
        var result = await _countsClient.StartAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to start count.";
        }
        else
        {
            TempData["Success"] = "Inventory count started.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid id, CompleteInventoryCountRequestViewModel model, CancellationToken cancellationToken)
    {
        var result = await _countsClient.CompleteAsync(id, model, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to complete count.";
        }
        else
        {
            TempData["Success"] = "Inventory count completed.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private bool HasCustomerContext()
        => !string.IsNullOrWhiteSpace(HttpContext.Session.GetStringValue(SessionKeys.CustomerId));
}
