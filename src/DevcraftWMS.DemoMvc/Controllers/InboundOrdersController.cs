using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.DemoMvc.ApiClients;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.InboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using DevcraftWMS.DemoMvc.Infrastructure;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class InboundOrdersController : Controller
{
    private readonly InboundOrdersApiClient _ordersClient;
    private readonly WarehousesApiClient _warehousesClient;

    public InboundOrdersController(
        InboundOrdersApiClient ordersClient,
        WarehousesApiClient warehousesClient)
    {
        _ordersClient = ordersClient;
        _warehousesClient = warehousesClient;
    }

    public async Task<IActionResult> Index([FromQuery] InboundOrderQuery query, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load inbound orders.";
            return View(new InboundOrderListPageViewModel
            {
                Query = query,
                Warehouses = Array.Empty<SelectListItem>(),
                Items = Array.Empty<InboundOrderListItemViewModel>()
            });
        }

        var listResult = await _ordersClient.ListAsync(query, cancellationToken);
        if (!listResult.IsSuccess)
        {
            TempData["Error"] = listResult.Error ?? "Unable to load inbound orders.";
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
            Controller = "InboundOrders",
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

        var model = new InboundOrderListPageViewModel
        {
            Query = query,
            Items = listResult.Data?.Items ?? Array.Empty<InboundOrderListItemViewModel>(),
            Pagination = pagination,
            Warehouses = warehouseOptions
        };

        return View(model);
    }

    private bool HasCustomerContext()
        => !string.IsNullOrWhiteSpace(HttpContext.Session.GetStringValue(SessionKeys.CustomerId));
}
