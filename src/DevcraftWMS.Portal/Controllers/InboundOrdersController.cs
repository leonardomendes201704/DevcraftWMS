using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Portal.ApiClients;
using DevcraftWMS.Portal.ViewModels.InboundOrders;
using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.Controllers;

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

    public async Task<IActionResult> Index([FromQuery] InboundOrderListQuery query, CancellationToken cancellationToken)
    {
        var listResult = await _ordersClient.ListAsync(query, cancellationToken);
        if (!listResult.IsSuccess)
        {
            TempData["Error"] = listResult.Error ?? "Unable to load inbound orders.";
        }

        var warehousesResult = await _warehousesClient.ListAsync(200, cancellationToken);
        var warehouses = warehousesResult.Data?.Items ?? Array.Empty<WarehouseOptionDto>();

        var model = new InboundOrderListViewModel
        {
            Query = query,
            Items = listResult.Data?.Items ?? Array.Empty<InboundOrderListItemDto>(),
            TotalCount = listResult.Data?.TotalCount ?? 0,
            Warehouses = warehouses
        };

        return View(model);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Inbound order not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new InboundOrderDetailViewModel
        {
            Order = result.Data,
            Items = result.Data.Items,
            Parameters = new UpdateInboundOrderParametersViewModel
            {
                InspectionLevel = result.Data.InspectionLevel,
                Priority = result.Data.Priority,
                SuggestedDock = result.Data.SuggestedDock
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> ConvertFromAsn(Guid asnId, string? notes, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.ConvertFromAsnAsync(new ConvertInboundOrderRequest(asnId, notes), cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to convert ASN to inbound order.";
            return RedirectToAction("Details", "Asns", new { id = asnId });
        }

        TempData["Success"] = "Inbound order created from ASN.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateParameters(Guid id, UpdateInboundOrderParametersViewModel model, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.UpdateParametersAsync(id,
            new UpdateInboundOrderParametersRequest(model.InspectionLevel, model.Priority, model.SuggestedDock),
            cancellationToken);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to update parameters.";
        }
        else
        {
            TempData["Success"] = "Parameters updated.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(Guid id, CancelInboundOrderViewModel model, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.CancelAsync(id, new CancelInboundOrderRequest(model.Reason), cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to cancel inbound order.";
        }
        else
        {
            TempData["Success"] = "Inbound order canceled.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Complete(Guid id, CompleteInboundOrderViewModel model, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.CompleteAsync(id, new CompleteInboundOrderRequest(model.AllowPartial, model.Notes), cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to complete inbound order.";
        }
        else
        {
            TempData["Success"] = "Inbound order closed.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
