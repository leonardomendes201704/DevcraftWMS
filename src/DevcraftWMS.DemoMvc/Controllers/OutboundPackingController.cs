using System.Text.Json;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.OutboundPacking;
using DevcraftWMS.DemoMvc.ViewModels.OutboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class OutboundPackingController : Controller
{
    private const string PackedPackagesTempDataKey = "OutboundPackedPackages";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly OutboundOrdersApiClient _ordersClient;
    private readonly OutboundPackingApiClient _packingClient;

    public OutboundPackingController(OutboundOrdersApiClient ordersClient, OutboundPackingApiClient packingClient)
    {
        _ordersClient = ordersClient;
        _packingClient = packingClient;
    }

    public async Task<IActionResult> Index([FromQuery] OutboundOrderQuery query, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load outbound orders.";
            return View(new OutboundPackingQueuePageViewModel
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
            Controller = "OutboundPacking",
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

        return View(new OutboundPackingQueuePageViewModel
        {
            Items = result.Data.Items,
            Query = query,
            Pagination = pagination
        });
    }

    public async Task<IActionResult> Details(Guid id, [FromQuery] int packages = 1, CancellationToken cancellationToken = default)
    {
        var result = await _ordersClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Outbound order not found.";
            return RedirectToAction(nameof(Index));
        }

        var packageCount = Math.Max(1, Math.Min(packages, 5));
        var packageInputs = BuildPackageInputs(result.Data, packageCount);
        var packedPackages = TryGetPackedPackages();

        return View(new OutboundPackingDetailsPageViewModel
        {
            Order = result.Data,
            Packing = new OutboundPackingSubmitViewModel { Packages = packageInputs },
            PackedPackages = packedPackages,
            PackageCount = packageCount
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pack(Guid id, OutboundPackingSubmitViewModel model, CancellationToken cancellationToken)
    {
        if (model.Packages.Count == 0)
        {
            TempData["Error"] = "Please add at least one package.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var packages = model.Packages.Select(p => new OutboundPackageRequestViewModel
        {
            PackageNumber = p.PackageNumber,
            WeightKg = p.WeightKg,
            LengthCm = p.LengthCm,
            WidthCm = p.WidthCm,
            HeightCm = p.HeightCm,
            Notes = p.Notes,
            Items = p.Items
                .Where(i => i.Quantity > 0)
                .Select(i => new OutboundPackageItemRequestViewModel
                {
                    OutboundOrderItemId = i.OutboundOrderItemId,
                    Quantity = i.Quantity
                })
                .ToList()
        }).ToList();

        var result = await _packingClient.RegisterAsync(id, new OutboundPackingRequestViewModel
        {
            Packages = packages
        }, cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to register packing.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData[PackedPackagesTempDataKey] = JsonSerializer.Serialize(result.Data.Select(MapPackageSummary).ToList(), JsonOptions);
        TempData["Success"] = "Packing registered successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private static List<OutboundPackageInputViewModel> BuildPackageInputs(OutboundOrderDetailViewModel order, int packageCount)
    {
        var list = new List<OutboundPackageInputViewModel>();
        for (var index = 0; index < packageCount; index++)
        {
            var package = new OutboundPackageInputViewModel
            {
                PackageNumber = $"PKG-{index + 1}",
                Items = order.Items.Select(item => new OutboundPackageItemInputViewModel
                {
                    OutboundOrderItemId = item.Id,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    UomCode = item.UomCode,
                    Quantity = index == 0 ? item.Quantity : 0m
                }).ToList()
            };

            list.Add(package);
        }

        return list;
    }

    private static OutboundPackageSummaryViewModel MapPackageSummary(OutboundPackageResponseViewModel package)
        => new()
        {
            PackageNumber = package.PackageNumber,
            WeightKg = package.WeightKg,
            LengthCm = package.LengthCm,
            WidthCm = package.WidthCm,
            HeightCm = package.HeightCm,
            Items = package.Items.Select(i => new OutboundPackageItemSummaryViewModel
            {
                ProductCode = i.ProductCode,
                ProductName = i.ProductName,
                UomCode = i.UomCode,
                Quantity = i.Quantity
            }).ToList()
        };

    private IReadOnlyList<OutboundPackageSummaryViewModel> TryGetPackedPackages()
    {
        if (!TempData.TryGetValue(PackedPackagesTempDataKey, out var value) || value is not string json)
        {
            return Array.Empty<OutboundPackageSummaryViewModel>();
        }

        var packages = JsonSerializer.Deserialize<IReadOnlyList<OutboundPackageSummaryViewModel>>(json, JsonOptions);
        return packages ?? Array.Empty<OutboundPackageSummaryViewModel>();
    }
}
