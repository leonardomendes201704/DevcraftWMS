using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.InventoryMovements;
using DevcraftWMS.DemoMvc.ViewModels.Locations;
using DevcraftWMS.DemoMvc.ViewModels.Products;
using DevcraftWMS.DemoMvc.ViewModels.Lots;
using DevcraftWMS.DemoMvc.ViewModels.Structures;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class InventoryMovementsController : Controller
{
    private readonly InventoryMovementsApiClient _movementsClient;
    private readonly StructuresApiClient _structuresClient;
    private readonly LocationsApiClient _locationsClient;
    private readonly ProductsApiClient _productsClient;
    private readonly LotsApiClient _lotsClient;

    public InventoryMovementsController(
        InventoryMovementsApiClient movementsClient,
        StructuresApiClient structuresClient,
        LocationsApiClient locationsClient,
        ProductsApiClient productsClient,
        LotsApiClient lotsClient)
    {
        _movementsClient = movementsClient;
        _structuresClient = structuresClient;
        _locationsClient = locationsClient;
        _productsClient = productsClient;
        _lotsClient = lotsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] InventoryMovementQuery query, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load inventory movements.";
            return View(new InventoryMovementListPageViewModel
            {
                Query = query
            });
        }

        query = query with
        {
            FromStructureId = query.FromStructureId is { } fromStructure && fromStructure == Guid.Empty ? null : query.FromStructureId,
            ToStructureId = query.ToStructureId is { } toStructure && toStructure == Guid.Empty ? null : query.ToStructureId,
            FromLocationId = query.FromLocationId is { } fromLocation && fromLocation == Guid.Empty ? null : query.FromLocationId,
            ToLocationId = query.ToLocationId is { } toLocation && toLocation == Guid.Empty ? null : query.ToLocationId,
            ProductId = query.ProductId is { } productId && productId == Guid.Empty ? null : query.ProductId,
            LotId = query.LotId is { } lotId && lotId == Guid.Empty ? null : query.LotId
        };

        var productOptions = await LoadProductOptionsAsync(query.ProductId, cancellationToken, includeAll: true);
        var fromStructureOptions = await LoadStructureOptionsAsync(query.FromStructureId, cancellationToken, includeAll: true);
        var toStructureOptions = await LoadStructureOptionsAsync(query.ToStructureId, cancellationToken, includeAll: true);
        var fromLocationOptions = await LoadLocationOptionsAsync(query.FromStructureId, query.FromLocationId, cancellationToken, includeAll: true);
        var toLocationOptions = await LoadLocationOptionsAsync(query.ToStructureId, query.ToLocationId, cancellationToken, includeAll: true);
        var lotOptions = query.ProductId.HasValue
            ? await LoadLotOptionsAsync(query.ProductId.Value, query.LotId, cancellationToken, includeAll: true)
            : BuildAllOptions("All lots");

        var result = await _movementsClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load inventory movements.";
            return View(new InventoryMovementListPageViewModel
            {
                Query = query,
                Products = productOptions,
                FromStructures = fromStructureOptions,
                ToStructures = toStructureOptions,
                FromLocations = fromLocationOptions,
                ToLocations = toLocationOptions,
                Lots = lotOptions
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "InventoryMovements",
            Query = new Dictionary<string, string?>
            {
                ["ProductId"] = query.ProductId?.ToString(),
                ["FromStructureId"] = query.FromStructureId?.ToString(),
                ["ToStructureId"] = query.ToStructureId?.ToString(),
                ["FromLocationId"] = query.FromLocationId?.ToString(),
                ["ToLocationId"] = query.ToLocationId?.ToString(),
                ["LotId"] = query.LotId?.ToString(),
                ["Status"] = query.Status?.ToString(),
                ["PerformedFromUtc"] = query.PerformedFromUtc?.ToString("yyyy-MM-dd"),
                ["PerformedToUtc"] = query.PerformedToUtc?.ToString("yyyy-MM-dd"),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new InventoryMovementListPageViewModel
        {
            Items = result.Data.Items,
            Pagination = pagination,
            Query = query,
            Products = productOptions,
            FromStructures = fromStructureOptions,
            ToStructures = toStructureOptions,
            FromLocations = fromLocationOptions,
            ToLocations = toLocationOptions,
            Lots = lotOptions
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _movementsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Inventory movement not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        Guid? selectFromStructureId,
        Guid? selectFromLocationId,
        Guid? selectToStructureId,
        Guid? selectToLocationId,
        Guid? selectProductId,
        Guid? selectLotId,
        CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "Select a customer first",
                Message = "Inventory movements require an active customer context. Select a customer to continue.",
                PrimaryActionText = "Go to customers",
                PrimaryActionUrl = Url.Action("Index", "Customers") ?? "#",
                SecondaryActionText = "Back to movements",
                SecondaryActionUrl = Url.Action("Index", "InventoryMovements") ?? "#",
                IconClass = "bi bi-person-check"
            };
            return View("DependencyPrompt", prompt);
        }

        var model = new InventoryMovementFormViewModel
        {
            FromStructureId = selectFromStructureId,
            ToStructureId = selectToStructureId,
            FromLocationId = selectFromLocationId ?? Guid.Empty,
            ToLocationId = selectToLocationId ?? Guid.Empty,
            ProductId = selectProductId ?? Guid.Empty,
            LotId = selectLotId
        };

        var formPrompt = await PopulateFormOptionsAsync(model, cancellationToken);
        if (formPrompt is not null)
        {
            return View("DependencyPrompt", formPrompt);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(InventoryMovementFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _movementsClient.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create inventory movement.");
            await PopulateFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Inventory movement created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    private async Task<DependencyPromptViewModel?> PopulateFormOptionsAsync(InventoryMovementFormViewModel model, CancellationToken cancellationToken)
    {
        var structures = await LoadStructureOptionsAsync(model.FromStructureId, cancellationToken, includeAll: false);
        var structuresForTo = await LoadStructureOptionsAsync(model.ToStructureId, cancellationToken, includeAll: false);
        if (structures.Count == 0)
        {
            return new DependencyPromptViewModel
            {
                Title = "No structure found",
                Message = "Inventory movements require structures and locations. Do you want to create a structure now?",
                PrimaryActionText = "Create structure",
                PrimaryActionUrl = Url.Action("Create", "Structures") ?? "#",
                SecondaryActionText = "Back to movements",
                SecondaryActionUrl = Url.Action("Index", "InventoryMovements") ?? "#",
                IconClass = "bi bi-stack"
            };
        }

        var products = await LoadProductOptionsAsync(model.ProductId, cancellationToken, includeAll: false);
        if (products.Count == 0)
        {
            return new DependencyPromptViewModel
            {
                Title = "No product found",
                Message = "Inventory movements require products. Do you want to create a product now?",
                PrimaryActionText = "Create product",
                PrimaryActionUrl = Url.Action("Create", "Products") ?? "#",
                SecondaryActionText = "Back to movements",
                SecondaryActionUrl = Url.Action("Index", "InventoryMovements") ?? "#",
                IconClass = "bi bi-upc"
            };
        }

        model.FromStructures = AddSelectPrompt(structures, model.FromStructureId.HasValue && model.FromStructureId != Guid.Empty);
        model.ToStructures = AddSelectPrompt(structuresForTo, model.ToStructureId.HasValue && model.ToStructureId != Guid.Empty);
        model.Products = AddSelectPrompt(products, model.ProductId != Guid.Empty);
        model.Lots = model.ProductId != Guid.Empty
            ? AddSelectPrompt(await LoadLotOptionsAsync(model.ProductId, model.LotId, cancellationToken, includeAll: false), model.LotId.HasValue)
            : BuildEmptyOptions();

        model.FromLocations = await LoadLocationOptionsAsync(model.FromStructureId, model.FromLocationId, cancellationToken, includeAll: false);
        model.ToLocations = await LoadLocationOptionsAsync(model.ToStructureId, model.ToLocationId, cancellationToken, includeAll: false);

        return null;
    }

    private bool HasCustomerContext()
    {
        var customerId = HttpContext.Session.GetStringValue(SessionKeys.CustomerId);
        return !string.IsNullOrWhiteSpace(customerId);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadStructureOptionsAsync(Guid? selectedId, CancellationToken cancellationToken, bool includeAll)
    {
        var result = await _structuresClient.ListForCustomerAsync(
            new StructureQuery(PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();

        if (includeAll)
        {
            options.Insert(0, new SelectListItem("All structures", Guid.Empty.ToString(), !selectedId.HasValue || selectedId == Guid.Empty));
        }

        return options;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadLocationOptionsAsync(Guid? structureId, Guid? selectedId, CancellationToken cancellationToken, bool includeAll)
    {
        if (!structureId.HasValue || structureId.Value == Guid.Empty)
        {
            return includeAll ? BuildAllOptions("All locations") : BuildEmptyOptions();
        }

        var result = await _locationsClient.ListAsync(
            new LocationQuery(StructureId: structureId.Value, PageNumber: 1, PageSize: 100, OrderBy: "Code", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return includeAll ? BuildAllOptions("All locations") : BuildEmptyOptions();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem(item.Code, item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();

        if (includeAll)
        {
            options.Insert(0, new SelectListItem("All locations", Guid.Empty.ToString(), !selectedId.HasValue || selectedId == Guid.Empty));
        }
        else
        {
            options = AddSelectPrompt(options, selectedId.HasValue && selectedId.Value != Guid.Empty).ToList();
        }

        return options;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadProductOptionsAsync(Guid? selectedId, CancellationToken cancellationToken, bool includeAll)
    {
        var result = await _productsClient.ListAsync(
            new ProductQuery(PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();

        if (includeAll)
        {
            options.Insert(0, new SelectListItem("All products", Guid.Empty.ToString(), !selectedId.HasValue || selectedId == Guid.Empty));
        }

        return options;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadLotOptionsAsync(Guid productId, Guid? selectedId, CancellationToken cancellationToken, bool includeAll)
    {
        var result = await _lotsClient.ListAsync(
            new LotQuery
            {
                ProductId = productId,
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "Code",
                OrderDir = "asc",
                IncludeInactive = false
            },
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return includeAll ? BuildAllOptions("All lots") : Array.Empty<SelectListItem>();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem(item.Code, item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();

        if (includeAll)
        {
            options.Insert(0, new SelectListItem("All lots", Guid.Empty.ToString(), !selectedId.HasValue || selectedId == Guid.Empty));
        }

        return options;
    }

    private static IReadOnlyList<SelectListItem> BuildEmptyOptions()
        => new List<SelectListItem> { new("Selecione...", string.Empty, true) };

    private static IReadOnlyList<SelectListItem> BuildAllOptions(string label)
        => new List<SelectListItem> { new(label, Guid.Empty.ToString(), true) };

    private static IReadOnlyList<SelectListItem> AddSelectPrompt(IReadOnlyList<SelectListItem> items, bool hasSelection)
    {
        var list = items.ToList();
        list.Insert(0, new SelectListItem("Selecione...", string.Empty, !hasSelection));
        return list;
    }
}
