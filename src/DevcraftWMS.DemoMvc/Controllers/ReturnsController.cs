using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.Returns;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class ReturnsController : Controller
{
    private readonly ReturnsApiClient _returnsClient;
    private readonly WarehousesApiClient _warehousesClient;

    public ReturnsController(ReturnsApiClient returnsClient, WarehousesApiClient warehousesClient)
    {
        _returnsClient = returnsClient;
        _warehousesClient = warehousesClient;
    }

    public async Task<IActionResult> Index([FromQuery] ReturnListQueryViewModel query, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load returns.";
            return View(new ReturnListPageViewModel
            {
                Query = query,
                Warehouses = Array.Empty<SelectListItem>(),
                Items = Array.Empty<ReturnOrderListItemViewModel>()
            });
        }

        var listResult = await _returnsClient.ListAsync(query, cancellationToken);
        if (!listResult.IsSuccess)
        {
            TempData["Error"] = listResult.Error ?? "Unable to load returns.";
        }

        var warehousesResult = await _warehousesClient.ListAsync(
            new WarehouseQuery(PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);
        var warehouses = warehousesResult.Data?.Items ?? Array.Empty<WarehouseListItemViewModel>();
        var warehouseOptions = warehouses
            .Select(wh => new SelectListItem($"{wh.Code} - {wh.Name}", wh.Id.ToString(), query.WarehouseId == wh.Id))
            .ToList();
        warehouseOptions.Insert(0, new SelectListItem("Select...", string.Empty, query.WarehouseId is null));

        if (!warehousesResult.IsSuccess)
        {
            TempData["Error"] = warehousesResult.Error ?? "Unable to load warehouses.";
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = listResult.Data?.PageNumber ?? query.PageNumber,
            PageSize = listResult.Data?.PageSize ?? query.PageSize,
            TotalCount = listResult.Data?.TotalCount ?? 0,
            Action = "Index",
            Controller = "Returns",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["ReturnNumber"] = query.ReturnNumber,
                ["Status"] = query.Status?.ToString(),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        var model = new ReturnListPageViewModel
        {
            Query = query,
            Items = listResult.Data?.Items ?? Array.Empty<ReturnOrderListItemViewModel>(),
            Pagination = pagination,
            Warehouses = warehouseOptions
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to view returns.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _returnsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Return not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new ReturnDetailsPageViewModel
        {
            Order = result.Data
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Receive(Guid id, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to receive returns.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _returnsClient.ReceiveAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to receive return.";
        }
        else
        {
            TempData["Success"] = "Return received.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid id, CompleteReturnOrderRequestViewModel model, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to complete returns.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _returnsClient.CompleteAsync(id, model, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to complete return.";
        }
        else
        {
            TempData["Success"] = "Return completed.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private bool HasCustomerContext()
        => !string.IsNullOrWhiteSpace(HttpContext.Session.GetStringValue(SessionKeys.CustomerId));
}
