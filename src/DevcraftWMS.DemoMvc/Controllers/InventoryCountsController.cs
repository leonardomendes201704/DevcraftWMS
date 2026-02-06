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
        query = NormalizeQuery(query);
        var customerId = HttpContext.Session.GetStringValue(SessionKeys.CustomerId);
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load inventory counts.";
            return View(new InventoryCountListPageViewModel { Query = query, DebugCustomerId = customerId });
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
            Pagination = pagination,
            DebugCustomerId = customerId,
            ApiStatusCode = result.StatusCode,
            ApiError = result.Error
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

    private static InventoryCountListQueryViewModel NormalizeQuery(InventoryCountListQueryViewModel query)
    {
        var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        if (pageSize > 200)
        {
            pageSize = 200;
        }

        var orderBy = string.IsNullOrWhiteSpace(query.OrderBy) ? "CreatedAtUtc" : query.OrderBy;
        var orderDir = string.IsNullOrWhiteSpace(query.OrderDir) ? "desc" : query.OrderDir;

        return query with
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDir = orderDir
        };
    }
}
