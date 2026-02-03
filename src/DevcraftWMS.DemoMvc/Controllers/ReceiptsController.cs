using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Receipts;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using DevcraftWMS.DemoMvc.ViewModels.Products;
using DevcraftWMS.DemoMvc.ViewModels.Lots;
using DevcraftWMS.DemoMvc.ViewModels.Sectors;
using DevcraftWMS.DemoMvc.ViewModels.Sections;
using DevcraftWMS.DemoMvc.ViewModels.Structures;
using DevcraftWMS.DemoMvc.ViewModels.Locations;
using DevcraftWMS.DemoMvc.ViewModels.Uoms;
using DevcraftWMS.DemoMvc.Infrastructure;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class ReceiptsController : Controller
{
    private readonly ReceiptsApiClient _receiptsClient;
    private readonly WarehousesApiClient _warehousesClient;
    private readonly ProductsApiClient _productsClient;
    private readonly LotsApiClient _lotsClient;
    private readonly SectorsApiClient _sectorsClient;
    private readonly SectionsApiClient _sectionsClient;
    private readonly StructuresApiClient _structuresClient;
    private readonly LocationsApiClient _locationsClient;
    private readonly UomsApiClient _uomsClient;

    public ReceiptsController(
        ReceiptsApiClient receiptsClient,
        WarehousesApiClient warehousesClient,
        ProductsApiClient productsClient,
        LotsApiClient lotsClient,
        SectorsApiClient sectorsClient,
        SectionsApiClient sectionsClient,
        StructuresApiClient structuresClient,
        LocationsApiClient locationsClient,
        UomsApiClient uomsClient)
    {
        _receiptsClient = receiptsClient;
        _warehousesClient = warehousesClient;
        _productsClient = productsClient;
        _lotsClient = lotsClient;
        _sectorsClient = sectorsClient;
        _sectionsClient = sectionsClient;
        _structuresClient = structuresClient;
        _locationsClient = locationsClient;
        _uomsClient = uomsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ReceiptQuery query, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load warehouses and receipts.";
            return View(new ReceiptListPageViewModel
            {
                Warehouses = Array.Empty<SelectListItem>(),
                Query = query
            });
        }

        var (warehouseOptions, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(query.WarehouseId, cancellationToken);
        if (!string.IsNullOrWhiteSpace(warehouseError))
        {
            TempData["Error"] = warehouseError;
        }
        else if (warehouseOptions.Count == 0)
        {
            TempData["Warning"] = "No warehouses available for the selected customer.";
        }

        var normalizedQuery = query with
        {
            WarehouseId = query.WarehouseId.HasValue && query.WarehouseId.Value == Guid.Empty ? null : query.WarehouseId
        };

        var result = await _receiptsClient.ListAsync(normalizedQuery, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load receipts.";
            return View(new ReceiptListPageViewModel
            {
                Warehouses = warehouseOptions,
                Query = normalizedQuery
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Receipts",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = normalizedQuery.WarehouseId?.ToString(),
                ["ReceiptNumber"] = normalizedQuery.ReceiptNumber,
                ["DocumentNumber"] = normalizedQuery.DocumentNumber,
                ["SupplierName"] = normalizedQuery.SupplierName,
                ["Status"] = normalizedQuery.Status?.ToString(),
                ["ReceivedFrom"] = normalizedQuery.ReceivedFrom?.ToString("yyyy-MM-dd"),
                ["ReceivedTo"] = normalizedQuery.ReceivedTo?.ToString("yyyy-MM-dd"),
                ["IsActive"] = normalizedQuery.IsActive?.ToString(),
                ["IncludeInactive"] = normalizedQuery.IncludeInactive.ToString(),
                ["OrderBy"] = normalizedQuery.OrderBy,
                ["OrderDir"] = normalizedQuery.OrderDir,
                ["PageSize"] = normalizedQuery.PageSize.ToString()
            }
        };

        var model = new ReceiptListPageViewModel
        {
            Items = result.Data.Items,
            Pagination = pagination,
            Query = normalizedQuery,
            Warehouses = warehouseOptions
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        Guid id,
        [FromQuery] ReceiptItemQuery query,
        Guid? selectSectorId,
        Guid? selectSectionId,
        Guid? selectStructureId,
        Guid? selectLocationId,
        Guid? selectProductId,
        Guid? selectLotId,
        Guid? selectUomId,
        CancellationToken cancellationToken)
    {
        var receiptResult = await _receiptsClient.GetByIdAsync(id, cancellationToken);
        if (!receiptResult.IsSuccess || receiptResult.Data is null)
        {
            TempData["Error"] = receiptResult.Error ?? "Receipt not found.";
            return RedirectToAction(nameof(Index));
        }

        var itemsResult = await _receiptsClient.ListItemsAsync(id, query, cancellationToken);
        if (!itemsResult.IsSuccess || itemsResult.Data is null)
        {
            TempData["Error"] = itemsResult.Error ?? "Failed to load receipt items.";
        }

        var newItem = new ReceiptItemFormViewModel
        {
            ReceiptId = id,
            SectorId = selectSectorId,
            SectionId = selectSectionId,
            StructureId = selectStructureId,
            LocationId = selectLocationId ?? Guid.Empty,
            ProductId = selectProductId ?? Guid.Empty,
            LotId = selectLotId,
            UomId = selectUomId ?? Guid.Empty
        };

        await PopulateItemOptionsAsync(receiptResult.Data.WarehouseId, newItem, cancellationToken);

        var pagination = new PaginationViewModel
        {
            PageNumber = itemsResult.Data?.PageNumber ?? query.PageNumber,
            PageSize = itemsResult.Data?.PageSize ?? query.PageSize,
            TotalCount = itemsResult.Data?.TotalCount ?? 0,
            Action = nameof(Details),
            Controller = "Receipts",
            Query = new Dictionary<string, string?>
            {
                ["id"] = id.ToString(),
                ["ProductId"] = query.ProductId?.ToString(),
                ["LocationId"] = query.LocationId?.ToString(),
                ["LotId"] = query.LotId?.ToString(),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["PageSize"] = query.PageSize.ToString(),
                ["selectSectorId"] = newItem.SectorId?.ToString(),
                ["selectSectionId"] = newItem.SectionId?.ToString(),
                ["selectStructureId"] = newItem.StructureId?.ToString(),
                ["selectLocationId"] = newItem.LocationId == Guid.Empty ? null : newItem.LocationId.ToString(),
                ["selectProductId"] = newItem.ProductId == Guid.Empty ? null : newItem.ProductId.ToString(),
                ["selectLotId"] = newItem.LotId?.ToString(),
                ["selectUomId"] = newItem.UomId == Guid.Empty ? null : newItem.UomId.ToString()
            }
        };

        var model = new ReceiptDetailsPageViewModel
        {
            Receipt = receiptResult.Data,
            Items = itemsResult.Data?.Items ?? Array.Empty<ReceiptItemListItemViewModel>(),
            Pagination = pagination,
            Query = query,
            NewItem = newItem
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Counts(Guid id, ReceiptCountMode mode = ReceiptCountMode.Blind, Guid? selectItemId = null, CancellationToken cancellationToken = default)
    {
        var receiptResult = await _receiptsClient.GetByIdAsync(id, cancellationToken);
        if (!receiptResult.IsSuccess || receiptResult.Data is null)
        {
            TempData["Error"] = receiptResult.Error ?? "Receipt not found.";
            return RedirectToAction(nameof(Index));
        }

        if (!receiptResult.Data.InboundOrderId.HasValue)
        {
            TempData["Error"] = "Receipt must be linked to an inbound order to count items.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var newCount = new ReceiptCountFormViewModel
        {
            ReceiptId = id,
            InboundOrderItemId = selectItemId ?? Guid.Empty,
            Mode = mode
        };

        var model = await BuildCountsViewModelAsync(receiptResult.Data, newCount, mode, cancellationToken);
        if (!model.ExpectedItems.Any())
        {
            TempData["Warning"] = "No inbound order items available for this receipt.";
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCount(ReceiptCountFormViewModel model, CancellationToken cancellationToken)
    {
        var receiptResult = await _receiptsClient.GetByIdAsync(model.ReceiptId, cancellationToken);
        if (!receiptResult.IsSuccess || receiptResult.Data is null)
        {
            TempData["Error"] = receiptResult.Error ?? "Receipt not found.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            var detailsModel = await BuildCountsViewModelAsync(receiptResult.Data, model, model.Mode, cancellationToken);
            return View("Counts", detailsModel);
        }

        var result = await _receiptsClient.RegisterCountAsync(model.ReceiptId, model, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to register count.");
            var detailsModel = await BuildCountsViewModelAsync(receiptResult.Data, model, model.Mode, cancellationToken);
            return View("Counts", detailsModel);
        }

        TempData["Success"] = "Count registered successfully.";
        return RedirectToAction(nameof(Counts), new { id = model.ReceiptId, mode = model.Mode });
    }

    [HttpPost]
    public async Task<IActionResult> AddItem([Bind(Prefix = "NewItem")] ReceiptItemFormViewModel model, CancellationToken cancellationToken)
    {
        var receiptResult = await _receiptsClient.GetByIdAsync(model.ReceiptId, cancellationToken);
        if (!receiptResult.IsSuccess || receiptResult.Data is null)
        {
            TempData["Error"] = receiptResult.Error ?? "Receipt not found.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            await PopulateItemOptionsAsync(receiptResult.Data.WarehouseId, model, cancellationToken);
            var detailsViewModel = await BuildDetailsViewModelAsync(receiptResult.Data, model, cancellationToken);
            return View("Details", detailsViewModel);
        }

        var result = await _receiptsClient.AddItemAsync(model.ReceiptId, model, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to add receipt item.");
            await PopulateItemOptionsAsync(receiptResult.Data.WarehouseId, model, cancellationToken);
            var detailsViewModel = await BuildDetailsViewModelAsync(receiptResult.Data, model, cancellationToken);
            return View("Details", detailsViewModel);
        }

        TempData["Success"] = "Receipt item added successfully.";
        return RedirectToAction(nameof(Details), new { id = model.ReceiptId });
    }

    [HttpPost]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _receiptsClient.CompleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to complete receipt.";
        }
        else
        {
            TempData["Success"] = "Receipt completed and inventory updated.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? warehouseId, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "Select a customer first",
                Message = "Receipts require an active customer context. Select a customer to continue.",
                PrimaryActionText = "Go to customers",
                PrimaryActionUrl = Url.Action("Index", "Customers") ?? "#",
                SecondaryActionText = "Back to receipts",
                SecondaryActionUrl = Url.Action("Index", "Receipts") ?? "#",
                IconClass = "bi bi-person-check"
            };
            return View("DependencyPrompt", prompt);
        }

        var (warehouses, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(warehouseId, cancellationToken);
        if (!string.IsNullOrWhiteSpace(warehouseError))
        {
            TempData["Error"] = warehouseError;
        }
        if (warehouses.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No warehouse found",
                Message = "Receipts require a warehouse. Do you want to create a warehouse now?",
                PrimaryActionText = "Create warehouse",
                PrimaryActionUrl = Url.Action("Create", "Warehouses") ?? "#",
                SecondaryActionText = "Back to receipts",
                SecondaryActionUrl = Url.Action("Index", "Receipts") ?? "#",
                IconClass = "bi bi-box-seam"
            };
            return View("DependencyPrompt", prompt);
        }

        var model = new ReceiptFormViewModel
        {
            WarehouseId = warehouseId ?? Guid.Empty,
            Warehouses = warehouses
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReceiptFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var (warehouses, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(model.WarehouseId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(warehouseError))
            {
                TempData["Error"] = warehouseError;
            }
            model.Warehouses = warehouses;
            return View(model);
        }

        var result = await _receiptsClient.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create receipt.");
            var (warehouses, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(model.WarehouseId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(warehouseError))
            {
                TempData["Error"] = warehouseError;
            }
            model.Warehouses = warehouses;
            return View(model);
        }

        TempData["Success"] = "Receipt created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _receiptsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Receipt not found.";
            return RedirectToAction(nameof(Index));
        }

        var (warehouses, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(result.Data.WarehouseId, cancellationToken);
        if (!string.IsNullOrWhiteSpace(warehouseError))
        {
            TempData["Error"] = warehouseError;
        }

        var model = new ReceiptFormViewModel
        {
            Id = result.Data.Id,
            WarehouseId = result.Data.WarehouseId,
            ReceiptNumber = result.Data.ReceiptNumber,
            DocumentNumber = result.Data.DocumentNumber,
            SupplierName = result.Data.SupplierName,
            Notes = result.Data.Notes,
            Warehouses = warehouses
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, ReceiptFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var (warehouses, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(model.WarehouseId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(warehouseError))
            {
                TempData["Error"] = warehouseError;
            }
            model.Warehouses = warehouses;
            return View(model);
        }

        var result = await _receiptsClient.UpdateAsync(id, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update receipt.");
            var (warehouses, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(model.WarehouseId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(warehouseError))
            {
                TempData["Error"] = warehouseError;
            }
            model.Warehouses = warehouses;
            return View(model);
        }

        TempData["Success"] = "Receipt updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _receiptsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Receipt not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _receiptsClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate receipt.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Receipt deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<ReceiptCountsPageViewModel> BuildCountsViewModelAsync(
        ReceiptDetailViewModel receipt,
        ReceiptCountFormViewModel newCount,
        ReceiptCountMode mode,
        CancellationToken cancellationToken)
    {
        var expectedResult = await _receiptsClient.ListExpectedItemsAsync(receipt.Id, cancellationToken);
        if (!expectedResult.IsSuccess)
        {
            TempData["Error"] = expectedResult.Error ?? "Failed to load expected items.";
        }

        var countsResult = await _receiptsClient.ListCountsAsync(receipt.Id, cancellationToken);
        if (!countsResult.IsSuccess)
        {
            TempData["Error"] = countsResult.Error ?? "Failed to load counts.";
        }

        var expectedItems = expectedResult.Data ?? Array.Empty<ReceiptExpectedItemViewModel>();
        var counts = countsResult.Data ?? Array.Empty<ReceiptCountListItemViewModel>();

        if (newCount.InboundOrderItemId == Guid.Empty && expectedItems.Count == 1)
        {
            newCount.InboundOrderItemId = expectedItems[0].InboundOrderItemId;
        }

        newCount.Items = BuildExpectedItemOptions(expectedItems, newCount.InboundOrderItemId);
        newCount.Mode = mode;
        newCount.ReceiptId = receipt.Id;

        return new ReceiptCountsPageViewModel
        {
            Receipt = receipt,
            ExpectedItems = expectedItems,
            Counts = counts,
            NewCount = newCount,
            Mode = mode
        };
    }

    private static IReadOnlyList<SelectListItem> BuildExpectedItemOptions(IReadOnlyList<ReceiptExpectedItemViewModel> items, Guid selectedId)
    {
        var options = items
            .Select(item => new SelectListItem(
                $"{item.ProductCode} - {item.ProductName} ({item.UomCode})",
                item.InboundOrderItemId.ToString(),
                item.InboundOrderItemId == selectedId))
            .ToList();

        return AddSelectPrompt(options, selectedId != Guid.Empty);
    }

    private async Task<ReceiptDetailsPageViewModel> BuildDetailsViewModelAsync(ReceiptDetailViewModel receipt, ReceiptItemFormViewModel newItem, CancellationToken cancellationToken)
    {
        var query = new ReceiptItemQuery();
        var itemsResult = await _receiptsClient.ListItemsAsync(receipt.Id, query, cancellationToken);

        var pagination = new PaginationViewModel
        {
            PageNumber = itemsResult.Data?.PageNumber ?? query.PageNumber,
            PageSize = itemsResult.Data?.PageSize ?? query.PageSize,
            TotalCount = itemsResult.Data?.TotalCount ?? 0,
            Action = nameof(Details),
            Controller = "Receipts",
            Query = new Dictionary<string, string?>
            {
                ["id"] = receipt.Id.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return new ReceiptDetailsPageViewModel
        {
            Receipt = receipt,
            Items = itemsResult.Data?.Items ?? Array.Empty<ReceiptItemListItemViewModel>(),
            Pagination = pagination,
            Query = query,
            NewItem = newItem
        };
    }

    private async Task PopulateItemOptionsAsync(Guid warehouseId, ReceiptItemFormViewModel model, CancellationToken cancellationToken)
    {
        model.Products = await LoadProductOptionsAsync(model.ProductId, cancellationToken);
        model.Uoms = await LoadUomOptionsAsync(model.UomId, cancellationToken);

        if (model.ProductId != Guid.Empty)
        {
            model.Lots = await LoadLotOptionsAsync(model.ProductId, model.LotId, cancellationToken);
        }
        else
        {
            model.Lots = BuildEmptyOptions();
        }

        var sectors = await LoadSectorOptionsAsync(warehouseId, model.SectorId, cancellationToken);
        model.Sectors = sectors;

        if (model.SectorId.HasValue)
        {
            model.Sections = await LoadSectionOptionsAsync(model.SectorId.Value, model.SectionId, cancellationToken);
        }
        else
        {
            model.Sections = BuildEmptyOptions();
        }

        if (model.SectionId.HasValue)
        {
            model.Structures = await LoadStructureOptionsAsync(model.SectionId.Value, model.StructureId, cancellationToken);
        }
        else
        {
            model.Structures = BuildEmptyOptions();
        }

        if (model.StructureId.HasValue)
        {
            model.Locations = await LoadLocationOptionsAsync(model.StructureId.Value, model.LocationId, cancellationToken);
        }
        else
        {
            model.Locations = BuildEmptyOptions();
        }
    }

    private async Task<(IReadOnlyList<SelectListItem> Options, string? Error)> LoadWarehouseOptionsWithErrorAsync(Guid? selectedId, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            return (Array.Empty<SelectListItem>(), "Select a customer to load warehouses.");
        }

        var result = await _warehousesClient.ListAsync(
            new WarehouseQuery(PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return (Array.Empty<SelectListItem>(), result.Error ?? "Unable to load warehouses.");
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedId.HasValue && item.Id == selectedId))
            .ToList();

        return (AddSelectPrompt(options, selectedId.HasValue), null);
    }

    private bool HasCustomerContext()
    {
        var customerId = HttpContext.Session.GetStringValue(SessionKeys.CustomerId);
        return !string.IsNullOrWhiteSpace(customerId);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadProductOptionsAsync(Guid selectedId, CancellationToken cancellationToken)
    {
        var result = await _productsClient.ListAsync(
            new ProductQuery(PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return BuildEmptyOptions();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), item.Id == selectedId))
            .ToList();

        return AddSelectPrompt(options, selectedId != Guid.Empty);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadLotOptionsAsync(Guid productId, Guid? selectedId, CancellationToken cancellationToken)
    {
        var query = new LotQuery
        {
            ProductId = productId,
            PageNumber = 1,
            PageSize = 100,
            OrderBy = "Code",
            OrderDir = "asc",
            IncludeInactive = false
        };

        var result = await _lotsClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return BuildEmptyOptions();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem(item.Code, item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();

        return AddSelectPrompt(options, selectedId.HasValue);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadUomOptionsAsync(Guid selectedId, CancellationToken cancellationToken)
    {
        var result = await _uomsClient.ListAsync(
            new UomQuery(PageNumber: 1, PageSize: 100, OrderBy: "Code", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return BuildEmptyOptions();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), item.Id == selectedId))
            .ToList();

        return AddSelectPrompt(options, selectedId != Guid.Empty);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadSectorOptionsAsync(Guid warehouseId, Guid? selectedId, CancellationToken cancellationToken)
    {
        var result = await _sectorsClient.ListAsync(
            new SectorQuery(WarehouseId: warehouseId, PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return BuildEmptyOptions();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();

        return AddSelectPrompt(options, selectedId.HasValue);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadSectionOptionsAsync(Guid sectorId, Guid? selectedId, CancellationToken cancellationToken)
    {
        var result = await _sectionsClient.ListAsync(
            new SectionQuery(SectorId: sectorId, PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return BuildEmptyOptions();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();

        return AddSelectPrompt(options, selectedId.HasValue);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadStructureOptionsAsync(Guid sectionId, Guid? selectedId, CancellationToken cancellationToken)
    {
        var result = await _structuresClient.ListAsync(
            new StructureQuery(SectionId: sectionId, PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return BuildEmptyOptions();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();

        return AddSelectPrompt(options, selectedId.HasValue);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadLocationOptionsAsync(Guid structureId, Guid selectedId, CancellationToken cancellationToken)
    {
        var result = await _locationsClient.ListAsync(
            new LocationQuery(StructureId: structureId, PageNumber: 1, PageSize: 100, OrderBy: "Code", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return BuildEmptyOptions();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem(item.Code, item.Id.ToString(), item.Id == selectedId))
            .ToList();

        return AddSelectPrompt(options, selectedId != Guid.Empty);
    }

    private static IReadOnlyList<SelectListItem> BuildEmptyOptions()
        => new List<SelectListItem> { new("Selecione...", string.Empty, true) };

    private static IReadOnlyList<SelectListItem> AddSelectPrompt(IReadOnlyList<SelectListItem> items, bool hasSelection)
    {
        var list = items.ToList();
        list.Insert(0, new SelectListItem("Selecione...", string.Empty, !hasSelection));
        return list;
    }
}
