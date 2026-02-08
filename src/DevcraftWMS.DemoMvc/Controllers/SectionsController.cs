using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Sections;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using DevcraftWMS.DemoMvc.ViewModels.Sectors;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class SectionsController : Controller
{
    private readonly SectionsApiClient _sectionsClient;
    private readonly WarehousesApiClient _warehousesClient;
    private readonly SectorsApiClient _sectorsClient;

    public SectionsController(SectionsApiClient sectionsClient, WarehousesApiClient warehousesClient, SectorsApiClient sectorsClient)
    {
        _sectionsClient = sectionsClient;
        _warehousesClient = warehousesClient;
        _sectorsClient = sectorsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] SectionQuery query, CancellationToken cancellationToken)
    {
        var warehouseOptions = await LoadWarehouseOptionsAsync(query.WarehouseId, cancellationToken);
        if (warehouseOptions.Count == 0)
        {
            TempData["Warning"] = "Create a warehouse before managing sections.";
            return View(new SectionListPageViewModel
            {
                Warehouses = warehouseOptions,
                Query = query
            });
        }

        var selectedWarehouseId = query.WarehouseId == Guid.Empty
            ? Guid.Parse(warehouseOptions[0].Value!)
            : query.WarehouseId;

        var sectorOptions = await LoadSectorOptionsAsync(selectedWarehouseId, query.SectorId, cancellationToken);
        if (sectorOptions.Count == 0)
        {
            TempData["Warning"] = "Create a sector before managing sections.";
            return View(new SectionListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Query = query with { WarehouseId = selectedWarehouseId }
            });
        }

        var selectedSectorId = query.SectorId == Guid.Empty
            ? Guid.Parse(sectorOptions[0].Value!)
            : query.SectorId;

        var normalizedQuery = query with { WarehouseId = selectedWarehouseId, SectorId = selectedSectorId };
        var result = await _sectionsClient.ListAsync(normalizedQuery, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load sections.";
            return View(new SectionListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Query = normalizedQuery
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Sections",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = normalizedQuery.WarehouseId.ToString(),
                ["SectorId"] = normalizedQuery.SectorId.ToString(),
                ["OrderBy"] = normalizedQuery.OrderBy,
                ["OrderDir"] = normalizedQuery.OrderDir,
                ["Code"] = normalizedQuery.Code,
                ["Name"] = normalizedQuery.Name,
                ["IsActive"] = normalizedQuery.IsActive?.ToString(),
                ["IncludeInactive"] = normalizedQuery.IncludeInactive.ToString(),
                ["PageSize"] = normalizedQuery.PageSize.ToString()
            }
        };

        var model = new SectionListPageViewModel
        {
            Items = result.Data.Items,
            Query = normalizedQuery,
            Pagination = pagination,
            Warehouses = warehouseOptions,
            Sectors = sectorOptions
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sectionsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Section not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? warehouseId, Guid? sectorId, CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehouseOptionsAsync(null, cancellationToken);
        if (warehouses.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No warehouse found",
                Message = "Sections depend on a warehouse and sector. Do you want to create a warehouse now?",
                PrimaryActionText = "Create warehouse",
                PrimaryActionUrl = Url.Action("Create", "Warehouses") ?? "#",
                SecondaryActionText = "Back to sections",
                SecondaryActionUrl = Url.Action("Index", "Sections") ?? "#",
                IconClass = "bi bi-box-seam"
            };
            return View("DependencyPrompt", prompt);
        }

        var selectedWarehouseId = warehouseId.HasValue && warehouses.Any(w => w.Value == warehouseId.Value.ToString())
            ? warehouseId.Value
            : Guid.Parse(warehouses[0].Value!);

        var sectors = await LoadSectorOptionsAsync(selectedWarehouseId, null, cancellationToken);
        if (sectors.Count == 0)
        {
            sectors = await LoadSectorOptionsAsync(null, null, cancellationToken);
        }

        if (sectors.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No sector found",
                Message = "Sections depend on a sector. Do you want to create a sector now?",
                PrimaryActionText = "Create sector",
                PrimaryActionUrl = Url.Action("Create", "Sectors", new { warehouseId = selectedWarehouseId }) ?? "#",
                SecondaryActionText = "Back to sections",
                SecondaryActionUrl = Url.Action("Index", "Sections", new { warehouseId = selectedWarehouseId }) ?? "#",
                IconClass = "bi bi-grid-3x3-gap"
            };
            return View("DependencyPrompt", prompt);
        }

        var selectedSectorId = sectorId.HasValue && sectors.Any(s => s.Value == sectorId.Value.ToString())
            ? sectorId.Value
            : Guid.Parse(sectors[0].Value!);

        var model = new SectionFormViewModel
        {
            WarehouseId = selectedWarehouseId,
            SectorId = selectedSectorId,
            Warehouses = warehouses,
            Sectors = sectors
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SectionFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSectionFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _sectionsClient.CreateAsync(model.SectorId, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create section.");
            await PopulateSectionFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Section created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sectionsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Section not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new SectionFormViewModel
        {
            Id = result.Data.Id,
            SectorId = result.Data.SectorId,
            Code = result.Data.Code,
            Name = result.Data.Name,
            Description = result.Data.Description
        };

        await PopulateSectionFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SectionFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            await PopulateSectionFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _sectionsClient.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update section.");
            await PopulateSectionFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Section updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sectionsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Section not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sectionsClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate section.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Section deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> SectorOptions(Guid warehouseId, CancellationToken cancellationToken)
    {
        var sectors = await LoadSectorOptionsAsync(warehouseId, null, cancellationToken);
        return Json(sectors.Select(item => new { value = item.Value ?? string.Empty, text = item.Text ?? string.Empty }));
    }

    private async Task PopulateSectionFormOptionsAsync(SectionFormViewModel model, CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehouseOptionsAsync(null, cancellationToken);
        var warehouseId = Guid.Empty;

        if (model.SectorId != Guid.Empty)
        {
            var sectorResult = await _sectorsClient.GetByIdAsync(model.SectorId, cancellationToken);
            if (sectorResult.IsSuccess && sectorResult.Data is not null)
            {
                warehouseId = sectorResult.Data.WarehouseId;
            }
        }

        if (warehouseId == Guid.Empty && warehouses.Count > 0)
        {
            warehouseId = Guid.Parse(warehouses[0].Value!);
        }

        var selectedSectorId = model.SectorId;
        var sectors = await LoadSectorOptionsAsync(
            warehouseId == Guid.Empty ? null : warehouseId,
            selectedSectorId,
            cancellationToken);

        model.WarehouseId = warehouseId == Guid.Empty ? model.WarehouseId : warehouseId;
        model.Warehouses = warehouses;
        model.Sectors = sectors;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadWarehouseOptionsAsync(Guid? selectedId, CancellationToken cancellationToken)
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

        return result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadSectorOptionsAsync(Guid? warehouseId, Guid? selectedSectorId, CancellationToken cancellationToken)
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

        var includeWarehouse = !warehouseId.HasValue || warehouseId.Value == Guid.Empty;

        return result.Data.Items
            .Select(item =>
            {
                var label = includeWarehouse
                    ? $"{item.WarehouseName} - {item.Code} - {item.Name}"
                    : $"{item.Code} - {item.Name}";

                return new SelectListItem(label, item.Id.ToString(), selectedSectorId.HasValue && item.Id == selectedSectorId.Value);
            })
            .ToList();
    }
}
