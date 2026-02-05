using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.OutboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class OutboundOrdersController : Controller
{
    private readonly OutboundOrdersApiClient _ordersClient;
    private readonly WarehousesApiClient _warehousesClient;

    public OutboundOrdersController(
        OutboundOrdersApiClient ordersClient,
        WarehousesApiClient warehousesClient)
    {
        _ordersClient = ordersClient;
        _warehousesClient = warehousesClient;
    }

    public async Task<IActionResult> Index([FromQuery] OutboundOrderQuery query, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load outbound orders.";
            return View(new OutboundOrderListPageViewModel
            {
                Query = query,
                Warehouses = Array.Empty<SelectListItem>(),
                Items = Array.Empty<OutboundOrderListItemViewModel>()
            });
        }

        var listResult = await _ordersClient.ListAsync(query, cancellationToken);
        if (!listResult.IsSuccess)
        {
            TempData["Error"] = listResult.Error ?? "Unable to load outbound orders.";
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
            Controller = "OutboundOrders",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["OrderNumber"] = query.OrderNumber,
                ["Status"] = query.Status?.ToString(),
                ["Priority"] = query.Priority?.ToString(),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        var model = new OutboundOrderListPageViewModel
        {
            Query = query,
            Items = listResult.Data?.Items ?? Array.Empty<OutboundOrderListItemViewModel>(),
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
            TempData["Warning"] = "Select a customer to view outbound orders.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _ordersClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Outbound order not found.";
            return RedirectToAction(nameof(Index));
        }

        var releaseModel = new OutboundOrderReleaseViewModel
        {
            Priority = result.Data.Priority,
            PickingMethod = result.Data.PickingMethod,
            ShippingWindowStartUtc = result.Data.ShippingWindowStartUtc,
            ShippingWindowEndUtc = result.Data.ShippingWindowEndUtc
        };

        return View(new OutboundOrderDetailsPageViewModel
        {
            Order = result.Data,
            Release = releaseModel,
            PriorityOptions = BuildPriorityOptions(result.Data.Priority),
            PickingMethodOptions = BuildPickingMethodOptions(result.Data.PickingMethod)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Release(
        Guid id,
        [Bind(Prefix = "Release")] OutboundOrderReleaseViewModel model,
        CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to release outbound orders.";
            return RedirectToAction(nameof(Index));
        }

        if (!model.Priority.HasValue)
        {
            TempData["Error"] = "Priority is required.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (!model.PickingMethod.HasValue)
        {
            TempData["Error"] = "Picking method is required.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (model.ShippingWindowStartUtc.HasValue ^ model.ShippingWindowEndUtc.HasValue)
        {
            TempData["Error"] = "Shipping window requires both start and end values.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (model.ShippingWindowStartUtc.HasValue && model.ShippingWindowEndUtc.HasValue &&
            model.ShippingWindowEndUtc.Value < model.ShippingWindowStartUtc.Value)
        {
            TempData["Error"] = "Shipping window end must be after start.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var payload = new
        {
            priority = model.Priority.Value,
            pickingMethod = model.PickingMethod.Value,
            shippingWindowStartUtc = model.ShippingWindowStartUtc,
            shippingWindowEndUtc = model.ShippingWindowEndUtc
        };

        var result = await _ordersClient.ReleaseAsync(id, payload, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to release outbound order.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Outbound order released.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private static IReadOnlyList<SelectListItem> BuildPriorityOptions(int? selected)
        => new List<SelectListItem>
        {
            new("Select...", string.Empty, !selected.HasValue),
            new("Low", "0", selected == 0),
            new("Normal", "1", selected == 1),
            new("High", "2", selected == 2),
            new("Urgent", "3", selected == 3)
        };

    private static IReadOnlyList<SelectListItem> BuildPickingMethodOptions(int? selected)
        => new List<SelectListItem>
        {
            new("Select...", string.Empty, !selected.HasValue),
            new("Single order", "0", selected == 0),
            new("Batch", "1", selected == 1),
            new("Wave", "2", selected == 2),
            new("Cluster", "3", selected == 3)
        };

    private bool HasCustomerContext()
        => !string.IsNullOrWhiteSpace(HttpContext.Session.GetStringValue(SessionKeys.CustomerId));
}
