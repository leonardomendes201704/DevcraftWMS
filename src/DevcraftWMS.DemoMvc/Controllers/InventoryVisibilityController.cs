using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.InventoryVisibility;
using DevcraftWMS.DemoMvc.ViewModels.Products;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class InventoryVisibilityController : Controller
{
    private readonly InventoryVisibilityApiClient _visibilityClient;
    private readonly WarehousesApiClient _warehousesClient;
    private readonly ProductsApiClient _productsClient;

    public InventoryVisibilityController(
        InventoryVisibilityApiClient visibilityClient,
        WarehousesApiClient warehousesClient,
        ProductsApiClient productsClient)
    {
        _visibilityClient = visibilityClient;
        _warehousesClient = warehousesClient;
        _productsClient = productsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] InventoryVisibilityQuery query, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load inventory visibility.";
            return View(new InventoryVisibilityPageViewModel
            {
                Query = query,
                HasCustomerContext = false
            });
        }

        var customerId = GetCustomerId();
        if (!customerId.HasValue)
        {
            TempData["Warning"] = "Select a customer to load inventory visibility.";
            return View(new InventoryVisibilityPageViewModel
            {
                Query = query,
                HasCustomerContext = false
            });
        }

        var (warehouseOptions, selectedWarehouseId) = await LoadWarehouseOptionsAsync(query.WarehouseId, cancellationToken, includeAll: false);
        if (warehouseOptions.Count == 0)
        {
            TempData["Warning"] = "Create a warehouse before running inventory visibility.";
            return View(new InventoryVisibilityPageViewModel
            {
                Warehouses = warehouseOptions,
                Query = query with { CustomerId = customerId },
                HasCustomerContext = true
            });
        }

        var normalizedQuery = query with
        {
            CustomerId = customerId,
            WarehouseId = selectedWarehouseId,
            ProductId = query.ProductId is { } productId && productId == Guid.Empty ? null : query.ProductId,
            Sku = string.IsNullOrWhiteSpace(query.Sku) ? null : query.Sku,
            LotCode = string.IsNullOrWhiteSpace(query.LotCode) ? null : query.LotCode
        };

        var productOptions = await LoadProductOptionsAsync(normalizedQuery.ProductId, cancellationToken, includeAll: true);

        var result = await _visibilityClient.GetAsync(normalizedQuery, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load inventory visibility.";
            return View(new InventoryVisibilityPageViewModel
            {
                Query = normalizedQuery,
                Warehouses = warehouseOptions,
                Products = productOptions,
                HasCustomerContext = true
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.Locations.PageNumber,
            PageSize = result.Data.Locations.PageSize,
            TotalCount = result.Data.Locations.TotalCount,
            Action = nameof(Index),
            Controller = "InventoryVisibility",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = normalizedQuery.WarehouseId?.ToString(),
                ["ProductId"] = normalizedQuery.ProductId?.ToString(),
                ["Sku"] = normalizedQuery.Sku,
                ["LotCode"] = normalizedQuery.LotCode,
                ["ExpirationFrom"] = normalizedQuery.ExpirationFrom?.ToString("yyyy-MM-dd"),
                ["ExpirationTo"] = normalizedQuery.ExpirationTo?.ToString("yyyy-MM-dd"),
                ["Status"] = normalizedQuery.Status?.ToString(),
                ["IsActive"] = normalizedQuery.IsActive?.ToString(),
                ["IncludeInactive"] = normalizedQuery.IncludeInactive.ToString(),
                ["OrderBy"] = normalizedQuery.OrderBy,
                ["OrderDir"] = normalizedQuery.OrderDir,
                ["PageSize"] = normalizedQuery.PageSize.ToString()
            }
        };

        IReadOnlyList<InventoryVisibilityTraceViewModel> traceItems = Array.Empty<InventoryVisibilityTraceViewModel>();
        if (normalizedQuery.ProductId.HasValue)
        {
            var timelineResult = await _visibilityClient.GetTimelineAsync(
                normalizedQuery.ProductId.Value,
                customerId.Value,
                normalizedQuery.WarehouseId ?? Guid.Empty,
                normalizedQuery.LotCode,
                null,
                cancellationToken);

            if (timelineResult.IsSuccess && timelineResult.Data is not null)
            {
                traceItems = timelineResult.Data;
            }
        }

        return View(new InventoryVisibilityPageViewModel
        {
            Query = normalizedQuery,
            Warehouses = warehouseOptions,
            Products = productOptions,
            SummaryItems = result.Data.Summary.Items,
            LocationItems = result.Data.Locations.Items,
            TraceItems = traceItems,
            Pagination = pagination,
            HasCustomerContext = true
        });
    }

    private bool HasCustomerContext()
        => !string.IsNullOrWhiteSpace(HttpContext.Session.GetStringValue(SessionKeys.CustomerId));

    private Guid? GetCustomerId()
    {
        var value = HttpContext.Session.GetStringValue(SessionKeys.CustomerId);
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }

    private async Task<(IReadOnlyList<SelectListItem> Items, Guid SelectedWarehouseId)> LoadWarehouseOptionsAsync(
        Guid? selectedId,
        CancellationToken cancellationToken,
        bool includeAll)
    {
        var result = await _warehousesClient.ListAsync(
            new WarehouseQuery(
                1,
                100,
                "Name",
                "asc",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return (Array.Empty<SelectListItem>(), Guid.Empty);
        }

        var warehouses = result.Data.Items;
        var effectiveSelectedId = ResolveSelectedWarehouseId(selectedId, warehouses);
        var items = warehouses
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), item.Id == effectiveSelectedId))
            .ToList();

        if (includeAll)
        {
            items.Insert(0, new SelectListItem("All warehouses", Guid.Empty.ToString(), !selectedId.HasValue || selectedId == Guid.Empty));
        }

        return (items, effectiveSelectedId);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadProductOptionsAsync(Guid? selectedProductId, CancellationToken cancellationToken, bool includeAll)
    {
        var result = await _productsClient.ListAsync(
            new ProductQuery(
                1,
                300,
                "Name",
                "asc",
                null,
                null,
                null,
                null,
                null,
                true,
                false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var items = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedProductId.HasValue && item.Id == selectedProductId.Value))
            .ToList();

        if (includeAll)
        {
            items.Insert(0, new SelectListItem("All products", Guid.Empty.ToString(), !selectedProductId.HasValue || selectedProductId == Guid.Empty));
        }

        return items;
    }

    private static Guid ResolveSelectedWarehouseId(Guid? candidate, IReadOnlyList<WarehouseListItemViewModel> options)
    {
        if (candidate.HasValue && candidate.Value != Guid.Empty && options.Any(option => option.Id == candidate.Value))
        {
            return candidate.Value;
        }

        var primary = options.FirstOrDefault(option => option.IsPrimary);
        if (primary is not null)
        {
            return primary.Id;
        }

        return options.Count > 0 ? options[0].Id : Guid.Empty;
    }
}
