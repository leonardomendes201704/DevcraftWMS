using DevcraftWMS.Portal.ApiClients;
using DevcraftWMS.Portal.ViewModels.OutboundOrders;
using DevcraftWMS.Portal.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Portal.Controllers;

public sealed class OutboundOrdersController : Controller
{
    private readonly OutboundOrdersApiClient _ordersClient;
    private readonly WarehousesApiClient _warehousesClient;
    private readonly ProductsApiClient _productsClient;
    private readonly UomsApiClient _uomsClient;

    public OutboundOrdersController(
        OutboundOrdersApiClient ordersClient,
        WarehousesApiClient warehousesClient,
        ProductsApiClient productsClient,
        UomsApiClient uomsClient)
    {
        _ordersClient = ordersClient;
        _warehousesClient = warehousesClient;
        _productsClient = productsClient;
        _uomsClient = uomsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] OutboundOrderListQuery query, CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehousesAsync(cancellationToken);
        var result = await _ordersClient.ListAsync(query, cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load outbound orders.";
        }

        var model = new OutboundOrderListViewModel
        {
            Query = query,
            Items = result.Data?.Items ?? Array.Empty<OutboundOrderListItemDto>(),
            TotalCount = result.Data?.TotalCount ?? 0,
            Warehouses = warehouses
        };

        ViewData["Title"] = "Outbound Orders";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehousesAsync(cancellationToken);
        var products = await LoadProductsAsync(cancellationToken);
        var uoms = await LoadUomsAsync(cancellationToken);

        if (warehouses.Count == 0)
        {
            TempData["Warning"] = "No warehouses available. Please contact backoffice to register a warehouse.";
        }

        var model = new OutboundOrderCreateViewModel
        {
            Warehouses = warehouses,
            Products = products,
            Uoms = uoms,
            Items = new List<OutboundOrderItemInputViewModel> { new() }
        };

        ViewData["Title"] = "Create Outbound Order";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OutboundOrderCreateViewModel model, CancellationToken cancellationToken)
    {
        model.Warehouses = await LoadWarehousesAsync(cancellationToken);
        model.Products = await LoadProductsAsync(cancellationToken);
        model.Uoms = await LoadUomsAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please review the fields and try again.";
            return View(model);
        }

        if (!model.WarehouseId.HasValue || model.WarehouseId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(model.WarehouseId), "Warehouse is required.");
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.OrderNumber))
        {
            ModelState.AddModelError(nameof(model.OrderNumber), "Order number is required.");
            return View(model);
        }

        if (model.Items.Count == 0 || model.Items.All(item => !item.ProductId.HasValue))
        {
            ModelState.AddModelError(string.Empty, "At least one item is required.");
            return View(model);
        }

        var items = model.Items
            .Where(item => item.ProductId.HasValue && item.UomId.HasValue && item.Quantity.HasValue)
            .Select(item => new
            {
                productId = item.ProductId!.Value,
                uomId = item.UomId!.Value,
                quantity = item.Quantity!.Value,
                lotCode = string.IsNullOrWhiteSpace(item.LotCode) ? null : item.LotCode.Trim(),
                expirationDate = item.ExpirationDate
            })
            .ToList();

        if (items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Each item must include product, UoM, and quantity.");
            return View(model);
        }

        var payload = new
        {
            warehouseId = model.WarehouseId.Value,
            orderNumber = model.OrderNumber.Trim(),
            customerReference = string.IsNullOrWhiteSpace(model.CustomerReference) ? null : model.CustomerReference.Trim(),
            carrierName = string.IsNullOrWhiteSpace(model.CarrierName) ? null : model.CarrierName.Trim(),
            expectedShipDate = model.ExpectedShipDate,
            notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim(),
            items
        };

        var result = await _ordersClient.CreateAsync(payload, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create outbound order.");
            return View(model);
        }

        TempData["Success"] = "Outbound order created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Outbound order not found.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = $"Order {result.Data.OrderNumber}";
        return View(new OutboundOrderDetailViewModel
        {
            Order = result.Data,
            Items = result.Data.Items
        });
    }

    private async Task<IReadOnlyList<WarehouseOptionDto>> LoadWarehousesAsync(CancellationToken cancellationToken)
    {
        var result = await _warehousesClient.ListAsync(100, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load warehouses.";
            return Array.Empty<WarehouseOptionDto>();
        }

        return result.Data.Items;
    }

    private async Task<IReadOnlyList<ProductOptionDto>> LoadProductsAsync(CancellationToken cancellationToken)
    {
        var result = await _productsClient.ListAsync(100, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load products.";
            return Array.Empty<ProductOptionDto>();
        }

        return result.Data.Items;
    }

    private async Task<IReadOnlyList<UomOptionDto>> LoadUomsAsync(CancellationToken cancellationToken)
    {
        var result = await _uomsClient.ListAsync(100, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load UoMs.";
            return Array.Empty<UomOptionDto>();
        }

        return result.Data.Items;
    }
}

