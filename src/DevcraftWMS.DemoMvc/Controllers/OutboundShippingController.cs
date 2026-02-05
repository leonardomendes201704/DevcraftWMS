using System.Text.Json;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.OutboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.OutboundShipping;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class OutboundShippingController : Controller
{
    private const string LastShipmentTempDataKey = "OutboundShippingLastShipment";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly OutboundOrdersApiClient _ordersClient;
    private readonly OutboundPackingApiClient _packingClient;
    private readonly OutboundShippingApiClient _shippingClient;

    public OutboundShippingController(
        OutboundOrdersApiClient ordersClient,
        OutboundPackingApiClient packingClient,
        OutboundShippingApiClient shippingClient)
    {
        _ordersClient = ordersClient;
        _packingClient = packingClient;
        _shippingClient = shippingClient;
    }

    public async Task<IActionResult> Index([FromQuery] OutboundOrderQuery query, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load outbound orders.";
            return View(new OutboundShippingQueuePageViewModel
            {
                Items = Array.Empty<OutboundOrderListItemViewModel>(),
                Query = query
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "OutboundShipping",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["OrderNumber"] = query.OrderNumber,
                ["Status"] = query.Status?.ToString(),
                ["Priority"] = query.Priority?.ToString(),
                ["CreatedFromUtc"] = query.CreatedFromUtc?.ToString("O"),
                ["CreatedToUtc"] = query.CreatedToUtc?.ToString("O"),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new OutboundShippingQueuePageViewModel
        {
            Items = result.Data.Items,
            Query = query,
            Pagination = pagination
        });
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var orderResult = await _ordersClient.GetByIdAsync(id, cancellationToken);
        if (!orderResult.IsSuccess || orderResult.Data is null)
        {
            TempData["Error"] = orderResult.Error ?? "Outbound order not found.";
            return RedirectToAction(nameof(Index));
        }

        var packagesResult = await _packingClient.ListByOrderIdAsync(id, cancellationToken);
        if (!packagesResult.IsSuccess || packagesResult.Data is null)
        {
            TempData["Error"] = packagesResult.Error ?? "Unable to load outbound packages.";
            return RedirectToAction(nameof(Index));
        }

        var packages = packagesResult.Data.Select(MapPackage).ToList();
        var shipping = new OutboundShippingSubmitViewModel
        {
            ShippedAtUtc = DateTime.UtcNow,
            Packages = packages.Select(p => new OutboundShipmentPackageInputViewModel
            {
                OutboundPackageId = p.OutboundPackageId,
                PackageNumber = p.PackageNumber,
                Selected = true
            }).ToList()
        };

        return View(new OutboundShippingDetailsPageViewModel
        {
            Order = orderResult.Data,
            Packages = packages,
            Shipping = shipping,
            LastShipment = TryGetLastShipment()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ship(Guid id, [Bind(Prefix = "Shipping")] OutboundShippingSubmitViewModel model, CancellationToken cancellationToken)
    {
        var selectedPackages = model.Packages.Where(p => p.Selected).ToList();
        if (selectedPackages.Count == 0)
        {
            TempData["Error"] = "Please select at least one package to ship.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (string.IsNullOrWhiteSpace(model.DockCode))
        {
            TempData["Error"] = "Dock code is required.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var request = new OutboundShipmentRequestViewModel(
            model.DockCode,
            model.LoadingStartedAtUtc,
            model.LoadingCompletedAtUtc,
            model.ShippedAtUtc,
            model.Notes,
            selectedPackages.Select(p => new OutboundShipmentPackageRequestViewModel(p.OutboundPackageId)).ToList());

        var result = await _shippingClient.RegisterAsync(id, request, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to register outbound shipment.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData[LastShipmentTempDataKey] = JsonSerializer.Serialize(result.Data.Items.Select(i => new OutboundShipmentItemSummaryViewModel
        {
            PackageNumber = i.PackageNumber,
            WeightKg = i.WeightKg
        }).ToList(), JsonOptions);

        TempData["Success"] = "Outbound shipment registered successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private static OutboundPackageShippingViewModel MapPackage(OutboundPackageResponseViewModel package)
        => new()
        {
            OutboundPackageId = package.Id,
            PackageNumber = package.PackageNumber,
            WeightKg = package.WeightKg,
            Items = package.Items.Select(i => new OutboundPackageShippingItemViewModel
            {
                ProductCode = i.ProductCode,
                ProductName = i.ProductName,
                UomCode = i.UomCode,
                Quantity = i.Quantity
            }).ToList()
        };

    private IReadOnlyList<OutboundShipmentItemSummaryViewModel> TryGetLastShipment()
    {
        if (!TempData.TryGetValue(LastShipmentTempDataKey, out var value) || value is not string json)
        {
            return Array.Empty<OutboundShipmentItemSummaryViewModel>();
        }

        var items = JsonSerializer.Deserialize<IReadOnlyList<OutboundShipmentItemSummaryViewModel>>(json, JsonOptions);
        return items ?? Array.Empty<OutboundShipmentItemSummaryViewModel>();
    }
}
