using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.InventoryBalances;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Locations;
using DevcraftWMS.DemoMvc.ViewModels.Products;
using DevcraftWMS.DemoMvc.ViewModels.Lots;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using DevcraftWMS.DemoMvc.ViewModels.Sectors;
using DevcraftWMS.DemoMvc.ViewModels.Sections;
using DevcraftWMS.DemoMvc.ViewModels.Structures;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class InventoryBalancesController : Controller
{
    private readonly InventoryBalancesApiClient _inventoryClient;
    private readonly WarehousesApiClient _warehousesClient;
    private readonly SectorsApiClient _sectorsClient;
    private readonly SectionsApiClient _sectionsClient;
    private readonly StructuresApiClient _structuresClient;
    private readonly LocationsApiClient _locationsClient;
    private readonly ProductsApiClient _productsClient;
    private readonly LotsApiClient _lotsClient;

    public InventoryBalancesController(
        InventoryBalancesApiClient inventoryClient,
        WarehousesApiClient warehousesClient,
        SectorsApiClient sectorsClient,
        SectionsApiClient sectionsClient,
        StructuresApiClient structuresClient,
        LocationsApiClient locationsClient,
        ProductsApiClient productsClient,
        LotsApiClient lotsClient)
    {
        _inventoryClient = inventoryClient;
        _warehousesClient = warehousesClient;
        _sectorsClient = sectorsClient;
        _sectionsClient = sectionsClient;
        _structuresClient = structuresClient;
        _locationsClient = locationsClient;
        _productsClient = productsClient;
        _lotsClient = lotsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] InventoryBalanceQuery query, CancellationToken cancellationToken)
    {
        var warehouseOptions = await LoadWarehouseOptionsAsync(query.WarehouseId, cancellationToken, includeAll: false);
        if (warehouseOptions.Count == 0)
        {
            TempData["Warning"] = "Create a warehouse before managing inventory balances.";
            return View(new InventoryBalanceListPageViewModel
            {
                Warehouses = warehouseOptions,
                Query = query
            });
        }

        var selectedWarehouseId = ResolveSelectedId(query.WarehouseId, warehouseOptions);
        var sectorOptions = await LoadSectorOptionsAsync(selectedWarehouseId, query.SectorId, cancellationToken, includeAll: false);
        if (sectorOptions.Count == 0)
        {
            TempData["Warning"] = "Create a sector before managing inventory balances.";
            query.WarehouseId = selectedWarehouseId;
            return View(new InventoryBalanceListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Query = query
            });
        }

        var selectedSectorId = ResolveSelectedId(query.SectorId, sectorOptions);
        var sectionOptions = await LoadSectionOptionsAsync(selectedSectorId, query.SectionId, cancellationToken, includeAll: false);
        if (sectionOptions.Count == 0)
        {
            TempData["Warning"] = "Create a section before managing inventory balances.";
            query.WarehouseId = selectedWarehouseId;
            query.SectorId = selectedSectorId;
            return View(new InventoryBalanceListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Sections = sectionOptions,
                Query = query
            });
        }

        var selectedSectionId = ResolveSelectedId(query.SectionId, sectionOptions);
        var structureOptions = await LoadStructureOptionsAsync(selectedSectionId, query.StructureId, cancellationToken, includeAll: false);
        if (structureOptions.Count == 0)
        {
            TempData["Warning"] = "Create a structure before managing inventory balances.";
            query.WarehouseId = selectedWarehouseId;
            query.SectorId = selectedSectorId;
            query.SectionId = selectedSectionId;
            return View(new InventoryBalanceListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Sections = sectionOptions,
                Structures = structureOptions,
                Query = query
            });
        }

        var selectedStructureId = ResolveSelectedId(query.StructureId, structureOptions);
        var locationOptions = await LoadLocationOptionsAsync(selectedStructureId, query.LocationId, cancellationToken, includeAll: true);
        if (locationOptions.Count == 0)
        {
            TempData["Warning"] = "Create a location before managing inventory balances.";
            query.WarehouseId = selectedWarehouseId;
            query.SectorId = selectedSectorId;
            query.SectionId = selectedSectionId;
            query.StructureId = selectedStructureId;
            return View(new InventoryBalanceListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Sections = sectionOptions,
                Structures = structureOptions,
                Locations = locationOptions,
                Query = query
            });
        }

        query.WarehouseId = selectedWarehouseId;
        query.SectorId = selectedSectorId;
        query.SectionId = selectedSectionId;
        query.StructureId = selectedStructureId;

        if (query.LocationId.HasValue && query.LocationId.Value == Guid.Empty)
        {
            query.LocationId = null;
        }

        if (query.ProductId.HasValue && query.ProductId.Value == Guid.Empty)
        {
            query.ProductId = null;
        }

        if (query.LotId.HasValue && query.LotId.Value == Guid.Empty)
        {
            query.LotId = null;
        }

        var productOptions = await LoadProductOptionsAsync(query.ProductId, cancellationToken, includeAll: true);
        var lotOptions = query.ProductId.HasValue
            ? await LoadLotOptionsAsync(query.ProductId.Value, query.LotId, cancellationToken, includeAll: true)
            : BuildEmptyOptions("All lots");

        var result = await _inventoryClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load inventory balances.";
            return View(new InventoryBalanceListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Sections = sectionOptions,
                Structures = structureOptions,
                Locations = locationOptions,
                Products = productOptions,
                Lots = lotOptions,
                Query = query
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "InventoryBalances",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["SectorId"] = query.SectorId?.ToString(),
                ["SectionId"] = query.SectionId?.ToString(),
                ["StructureId"] = query.StructureId?.ToString(),
                ["LocationId"] = query.LocationId?.ToString(),
                ["ProductId"] = query.ProductId?.ToString(),
                ["LotId"] = query.LotId?.ToString(),
                ["Status"] = query.Status?.ToString(),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new InventoryBalanceListPageViewModel
        {
            Items = result.Data.Items,
            Query = query,
            Pagination = pagination,
            Warehouses = warehouseOptions,
            Sectors = sectorOptions,
            Sections = sectionOptions,
            Structures = structureOptions,
            Locations = locationOptions,
            Products = productOptions,
            Lots = lotOptions
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventoryClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Inventory balance not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? warehouseId, Guid? sectorId, Guid? sectionId, Guid? structureId, Guid? locationId, Guid? productId, Guid? lotId, CancellationToken cancellationToken)
    {
        var model = new InventoryBalanceFormViewModel
        {
            WarehouseId = warehouseId ?? Guid.Empty,
            SectorId = sectorId ?? Guid.Empty,
            SectionId = sectionId ?? Guid.Empty,
            StructureId = structureId ?? Guid.Empty,
            LocationId = locationId ?? Guid.Empty,
            ProductId = productId ?? Guid.Empty,
            LotId = lotId
        };

        var prompt = await PopulateInventoryFormOptionsAsync(model, cancellationToken, requireLocation: true, requireProduct: true);
        if (prompt is not null)
        {
            return View("DependencyPrompt", prompt);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(InventoryBalanceFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateInventoryFormOptionsAsync(model, cancellationToken, requireLocation: false, requireProduct: false);
            return View(model);
        }

        var result = await _inventoryClient.CreateAsync(model.LocationId, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create inventory balance.");
            await PopulateInventoryFormOptionsAsync(model, cancellationToken, requireLocation: false, requireProduct: false);
            return View(model);
        }

        TempData["Success"] = "Inventory balance created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventoryClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Inventory balance not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new InventoryBalanceFormViewModel
        {
            Id = result.Data.Id,
            LocationId = result.Data.LocationId,
            ProductId = result.Data.ProductId,
            LotId = result.Data.LotId,
            QuantityOnHand = result.Data.QuantityOnHand,
            QuantityReserved = result.Data.QuantityReserved,
            Status = result.Data.Status
        };

        await PopulateInventoryFormOptionsAsync(model, cancellationToken, requireLocation: false, requireProduct: false);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(InventoryBalanceFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            await PopulateInventoryFormOptionsAsync(model, cancellationToken, requireLocation: false, requireProduct: false);
            return View(model);
        }

        var result = await _inventoryClient.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update inventory balance.");
            await PopulateInventoryFormOptionsAsync(model, cancellationToken, requireLocation: false, requireProduct: false);
            return View(model);
        }

        TempData["Success"] = "Inventory balance updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventoryClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Inventory balance not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventoryClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate inventory balance.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Inventory balance deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<DependencyPromptViewModel?> PopulateInventoryFormOptionsAsync(InventoryBalanceFormViewModel model, CancellationToken cancellationToken, bool requireLocation, bool requireProduct)
    {
        if (model.LocationId != Guid.Empty)
        {
            var locationResult = await _locationsClient.GetByIdAsync(model.LocationId, cancellationToken);
            if (locationResult.IsSuccess && locationResult.Data is not null)
            {
                model.StructureId = locationResult.Data.StructureId;
            }
        }

        if (model.StructureId != Guid.Empty)
        {
            var structureResult = await _structuresClient.GetByIdAsync(model.StructureId, cancellationToken);
            if (structureResult.IsSuccess && structureResult.Data is not null)
            {
                model.SectionId = structureResult.Data.SectionId;
            }
        }

        if (model.SectionId != Guid.Empty)
        {
            var sectionResult = await _sectionsClient.GetByIdAsync(model.SectionId, cancellationToken);
            if (sectionResult.IsSuccess && sectionResult.Data is not null)
            {
                model.SectorId = sectionResult.Data.SectorId;
            }
        }

        if (model.SectorId != Guid.Empty)
        {
            var sectorResult = await _sectorsClient.GetByIdAsync(model.SectorId, cancellationToken);
            if (sectorResult.IsSuccess && sectorResult.Data is not null)
            {
                model.WarehouseId = sectorResult.Data.WarehouseId;
            }
        }

        model.Warehouses = await LoadWarehouseOptionsAsync(model.WarehouseId, cancellationToken, includeAll: false);
        if (model.Warehouses.Count == 0)
        {
            return new DependencyPromptViewModel
            {
                Title = "No warehouse found",
                Message = "Inventory balances depend on warehouse structure. Do you want to create a warehouse now?",
                PrimaryActionText = "Create warehouse",
                PrimaryActionUrl = Url.Action("Create", "Warehouses") ?? "#",
                SecondaryActionText = "Back to inventory",
                SecondaryActionUrl = Url.Action("Index", "InventoryBalances") ?? "#",
                IconClass = "bi bi-box-seam"
            };
        }

        if (model.WarehouseId == Guid.Empty)
        {
            model.WarehouseId = Guid.Parse(model.Warehouses[0].Value!);
        }

        model.Sectors = await LoadSectorOptionsAsync(model.WarehouseId, model.SectorId, cancellationToken, includeAll: false);
        if (model.Sectors.Count == 0)
        {
            return new DependencyPromptViewModel
            {
                Title = "No sector found",
                Message = "Inventory balances depend on sectors. Do you want to create a sector now?",
                PrimaryActionText = "Create sector",
                PrimaryActionUrl = Url.Action("Create", "Sectors", new { warehouseId = model.WarehouseId }) ?? "#",
                SecondaryActionText = "Back to inventory",
                SecondaryActionUrl = Url.Action("Index", "InventoryBalances") ?? "#",
                IconClass = "bi bi-grid-3x3-gap"
            };
        }

        if (model.SectorId == Guid.Empty)
        {
            model.SectorId = Guid.Parse(model.Sectors[0].Value!);
        }

        model.Sections = await LoadSectionOptionsAsync(model.SectorId, model.SectionId, cancellationToken, includeAll: false);
        if (model.Sections.Count == 0)
        {
            return new DependencyPromptViewModel
            {
                Title = "No section found",
                Message = "Inventory balances depend on sections. Do you want to create a section now?",
                PrimaryActionText = "Create section",
                PrimaryActionUrl = Url.Action("Create", "Sections", new { warehouseId = model.WarehouseId, sectorId = model.SectorId }) ?? "#",
                SecondaryActionText = "Back to inventory",
                SecondaryActionUrl = Url.Action("Index", "InventoryBalances") ?? "#",
                IconClass = "bi bi-layout-text-sidebar"
            };
        }

        if (model.SectionId == Guid.Empty)
        {
            model.SectionId = Guid.Parse(model.Sections[0].Value!);
        }

        model.Structures = await LoadStructureOptionsAsync(model.SectionId, model.StructureId, cancellationToken, includeAll: false);
        if (model.Structures.Count == 0)
        {
            return new DependencyPromptViewModel
            {
                Title = "No structure found",
                Message = "Inventory balances depend on structures. Do you want to create a structure now?",
                PrimaryActionText = "Create structure",
                PrimaryActionUrl = Url.Action("Create", "Structures", new { warehouseId = model.WarehouseId, sectorId = model.SectorId, sectionId = model.SectionId }) ?? "#",
                SecondaryActionText = "Back to inventory",
                SecondaryActionUrl = Url.Action("Index", "InventoryBalances") ?? "#",
                IconClass = "bi bi-stack"
            };
        }

        if (model.StructureId == Guid.Empty)
        {
            model.StructureId = Guid.Parse(model.Structures[0].Value!);
        }

        model.Locations = await LoadLocationOptionsAsync(model.StructureId, model.LocationId, cancellationToken, includeAll: !requireLocation);
        if (requireLocation && model.Locations.Count == 0)
        {
            return new DependencyPromptViewModel
            {
                Title = "No location found",
                Message = "Inventory balances depend on locations. Do you want to create a location now?",
                PrimaryActionText = "Create location",
                PrimaryActionUrl = Url.Action("Create", "Locations", new { warehouseId = model.WarehouseId, sectorId = model.SectorId, sectionId = model.SectionId, structureId = model.StructureId }) ?? "#",
                SecondaryActionText = "Back to inventory",
                SecondaryActionUrl = Url.Action("Index", "InventoryBalances") ?? "#",
                IconClass = "bi bi-geo-alt"
            };
        }

        if (requireLocation && model.LocationId == Guid.Empty && model.Locations.Count > 0)
        {
            model.LocationId = Guid.Parse(model.Locations[0].Value!);
        }

        model.Products = await LoadProductOptionsAsync(model.ProductId, cancellationToken, includeAll: !requireProduct);
        if (requireProduct && model.Products.Count == 0)
        {
            return new DependencyPromptViewModel
            {
                Title = "No product found",
                Message = "Inventory balances depend on products. Do you want to create a product now?",
                PrimaryActionText = "Create product",
                PrimaryActionUrl = Url.Action("Create", "Products") ?? "#",
                SecondaryActionText = "Back to inventory",
                SecondaryActionUrl = Url.Action("Index", "InventoryBalances") ?? "#",
                IconClass = "bi bi-upc"
            };
        }

        if (requireProduct && model.ProductId == Guid.Empty && model.Products.Count > 0)
        {
            model.ProductId = Guid.Parse(model.Products[0].Value!);
        }

        model.Lots = model.ProductId != Guid.Empty
            ? await LoadLotOptionsAsync(model.ProductId, model.LotId, cancellationToken, includeAll: true)
            : BuildEmptyOptions("No lots");

        return null;
    }

    private static Guid ResolveSelectedId(Guid? candidate, IReadOnlyList<SelectListItem> options)
    {
        if (candidate.HasValue && candidate.Value != Guid.Empty && options.Any(option => option.Value == candidate.Value.ToString()))
        {
            return candidate.Value;
        }

        return Guid.Parse(options[0].Value!);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadWarehouseOptionsAsync(Guid? selectedId, CancellationToken cancellationToken, bool includeAll)
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
            return Array.Empty<SelectListItem>();
        }

        var items = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();

        if (includeAll)
        {
            items.Insert(0, new SelectListItem("All warehouses", Guid.Empty.ToString(), !selectedId.HasValue || selectedId == Guid.Empty));
        }

        return items;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadSectorOptionsAsync(Guid warehouseId, Guid? selectedSectorId, CancellationToken cancellationToken, bool includeAll)
    {
        var result = await _sectorsClient.ListAsync(
            new SectorQuery(
                warehouseId,
                1,
                100,
                "Name",
                "asc",
                null,
                null,
                null,
                null,
                false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var items = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedSectorId.HasValue && item.Id == selectedSectorId.Value))
            .ToList();

        if (includeAll)
        {
            items.Insert(0, new SelectListItem("All sectors", Guid.Empty.ToString(), !selectedSectorId.HasValue || selectedSectorId == Guid.Empty));
        }

        return items;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadSectionOptionsAsync(Guid sectorId, Guid? selectedSectionId, CancellationToken cancellationToken, bool includeAll)
    {
        var result = await _sectionsClient.ListAsync(
            new SectionQuery(
                Guid.Empty,
                sectorId,
                1,
                100,
                "Name",
                "asc",
                null,
                null,
                null,
                false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var items = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedSectionId.HasValue && item.Id == selectedSectionId.Value))
            .ToList();

        if (includeAll)
        {
            items.Insert(0, new SelectListItem("All sections", Guid.Empty.ToString(), !selectedSectionId.HasValue || selectedSectionId == Guid.Empty));
        }

        return items;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadStructureOptionsAsync(Guid sectionId, Guid? selectedStructureId, CancellationToken cancellationToken, bool includeAll)
    {
        var result = await _structuresClient.ListAsync(
            new StructureQuery(
                Guid.Empty,
                Guid.Empty,
                sectionId,
                1,
                100,
                "Name",
                "asc",
                null,
                null,
                null,
                null,
                false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var items = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedStructureId.HasValue && item.Id == selectedStructureId.Value))
            .ToList();

        if (includeAll)
        {
            items.Insert(0, new SelectListItem("All structures", Guid.Empty.ToString(), !selectedStructureId.HasValue || selectedStructureId == Guid.Empty));
        }

        return items;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadLocationOptionsAsync(Guid structureId, Guid? selectedLocationId, CancellationToken cancellationToken, bool includeAll)
    {
        if (structureId == Guid.Empty)
        {
            return Array.Empty<SelectListItem>();
        }

        var result = await _locationsClient.ListAsync(
            new LocationQuery(
                Guid.Empty,
                Guid.Empty,
                Guid.Empty,
                structureId,
                null,
                1,
                200,
                "Code",
                "asc",
                null,
                null,
                null,
                false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var items = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} (L{item.Level}-R{item.Row}-C{item.Column})", item.Id.ToString(), selectedLocationId.HasValue && item.Id == selectedLocationId.Value))
            .ToList();

        if (includeAll)
        {
            items.Insert(0, new SelectListItem("All locations", Guid.Empty.ToString(), !selectedLocationId.HasValue || selectedLocationId == Guid.Empty));
        }

        return items;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadProductOptionsAsync(Guid? selectedProductId, CancellationToken cancellationToken, bool includeAll)
    {
        var result = await _productsClient.ListAsync(
            new ProductQuery(
                1,
                200,
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

    private async Task<IReadOnlyList<SelectListItem>> LoadLotOptionsAsync(Guid productId, Guid? selectedLotId, CancellationToken cancellationToken, bool includeAll)
    {
        var result = await _lotsClient.ListAsync(
            new LotQuery
            {
                ProductId = productId,
                PageNumber = 1,
                PageSize = 200,
                OrderBy = "Code",
                OrderDir = "asc",
                IncludeInactive = false
            },
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var items = result.Data.Items
            .Select(item => new SelectListItem(item.Code, item.Id.ToString(), selectedLotId.HasValue && item.Id == selectedLotId.Value))
            .ToList();

        if (includeAll)
        {
            items.Insert(0, new SelectListItem("All lots", Guid.Empty.ToString(), !selectedLotId.HasValue || selectedLotId == Guid.Empty));
        }

        return items;
    }

    private static IReadOnlyList<SelectListItem> BuildEmptyOptions(string label)
        => new[] { new SelectListItem(label, Guid.Empty.ToString(), true) };
}
