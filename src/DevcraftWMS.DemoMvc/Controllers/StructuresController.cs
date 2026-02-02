using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Structures;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using DevcraftWMS.DemoMvc.ViewModels.Sectors;
using DevcraftWMS.DemoMvc.ViewModels.Sections;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class StructuresController : Controller
{
    private readonly StructuresApiClient _structuresClient;
    private readonly WarehousesApiClient _warehousesClient;
    private readonly SectorsApiClient _sectorsClient;
    private readonly SectionsApiClient _sectionsClient;

    public StructuresController(
        StructuresApiClient structuresClient,
        WarehousesApiClient warehousesClient,
        SectorsApiClient sectorsClient,
        SectionsApiClient sectionsClient)
    {
        _structuresClient = structuresClient;
        _warehousesClient = warehousesClient;
        _sectorsClient = sectorsClient;
        _sectionsClient = sectionsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] StructureQuery query, CancellationToken cancellationToken)
    {
        var warehouseOptions = await LoadWarehouseOptionsAsync(query.WarehouseId, cancellationToken);
        if (warehouseOptions.Count == 0)
        {
            TempData["Warning"] = "Create a warehouse before managing structures.";
            return View(new StructureListPageViewModel
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
            TempData["Warning"] = "Create a sector before managing structures.";
            return View(new StructureListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Query = query with { WarehouseId = selectedWarehouseId }
            });
        }

        var selectedSectorId = query.SectorId == Guid.Empty
            ? Guid.Parse(sectorOptions[0].Value!)
            : query.SectorId;

        var sectionOptions = await LoadSectionOptionsAsync(selectedSectorId, query.SectionId, cancellationToken);
        if (sectionOptions.Count == 0)
        {
            TempData["Warning"] = "Create a section before managing structures.";
            return View(new StructureListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Sections = sectionOptions,
                Query = query with { WarehouseId = selectedWarehouseId, SectorId = selectedSectorId }
            });
        }

        var selectedSectionId = query.SectionId == Guid.Empty
            ? Guid.Parse(sectionOptions[0].Value!)
            : query.SectionId;

        var normalizedQuery = query with { WarehouseId = selectedWarehouseId, SectorId = selectedSectorId, SectionId = selectedSectionId };
        var result = await _structuresClient.ListAsync(normalizedQuery, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load structures.";
            return View(new StructureListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Sections = sectionOptions,
                Query = normalizedQuery
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Structures",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = normalizedQuery.WarehouseId.ToString(),
                ["SectorId"] = normalizedQuery.SectorId.ToString(),
                ["SectionId"] = normalizedQuery.SectionId.ToString(),
                ["OrderBy"] = normalizedQuery.OrderBy,
                ["OrderDir"] = normalizedQuery.OrderDir,
                ["Code"] = normalizedQuery.Code,
                ["Name"] = normalizedQuery.Name,
                ["StructureType"] = normalizedQuery.StructureType?.ToString(),
                ["IsActive"] = normalizedQuery.IsActive?.ToString(),
                ["IncludeInactive"] = normalizedQuery.IncludeInactive.ToString(),
                ["PageSize"] = normalizedQuery.PageSize.ToString()
            }
        };

        var model = new StructureListPageViewModel
        {
            Items = result.Data.Items,
            Query = normalizedQuery,
            Pagination = pagination,
            Warehouses = warehouseOptions,
            Sectors = sectorOptions,
            Sections = sectionOptions
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _structuresClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Structure not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? warehouseId, Guid? sectorId, Guid? sectionId, CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehouseOptionsAsync(null, cancellationToken);
        if (warehouses.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No warehouse found",
                Message = "Structures depend on a warehouse, sector, and section. Do you want to create a warehouse now?",
                PrimaryActionText = "Create warehouse",
                PrimaryActionUrl = Url.Action("Create", "Warehouses") ?? "#",
                SecondaryActionText = "Back to structures",
                SecondaryActionUrl = Url.Action("Index", "Structures") ?? "#",
                IconClass = "bi bi-box-seam"
            };
            return View("DependencyPrompt", prompt);
        }

        var selectedWarehouseId = warehouseId.HasValue && warehouses.Any(w => w.Value == warehouseId.Value.ToString())
            ? warehouseId.Value
            : Guid.Parse(warehouses[0].Value!);

        var sectors = await LoadSectorOptionsAsync(selectedWarehouseId, sectorId, cancellationToken);
        if (sectors.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No sector found",
                Message = "Structures depend on a sector and section. Do you want to create a sector now?",
                PrimaryActionText = "Create sector",
                PrimaryActionUrl = Url.Action("Create", "Sectors", new { warehouseId = selectedWarehouseId }) ?? "#",
                SecondaryActionText = "Back to structures",
                SecondaryActionUrl = Url.Action("Index", "Structures", new { warehouseId = selectedWarehouseId }) ?? "#",
                IconClass = "bi bi-grid-3x3-gap"
            };
            return View("DependencyPrompt", prompt);
        }

        var selectedSectorId = sectorId.HasValue && sectors.Any(s => s.Value == sectorId.Value.ToString())
            ? sectorId.Value
            : Guid.Parse(sectors[0].Value!);

        var sections = await LoadSectionOptionsAsync(selectedSectorId, sectionId, cancellationToken);
        if (sections.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No section found",
                Message = "Structures depend on a section. Do you want to create a section now?",
                PrimaryActionText = "Create section",
                PrimaryActionUrl = Url.Action("Create", "Sections", new { warehouseId = selectedWarehouseId, sectorId = selectedSectorId }) ?? "#",
                SecondaryActionText = "Back to structures",
                SecondaryActionUrl = Url.Action("Index", "Structures", new { warehouseId = selectedWarehouseId, sectorId = selectedSectorId }) ?? "#",
                IconClass = "bi bi-layout-text-sidebar"
            };
            return View("DependencyPrompt", prompt);
        }

        var selectedSectionId = sectionId.HasValue && sections.Any(s => s.Value == sectionId.Value.ToString())
            ? sectionId.Value
            : Guid.Parse(sections[0].Value!);

        var model = new StructureFormViewModel
        {
            SectionId = selectedSectionId,
            Warehouses = warehouses,
            Sectors = sectors,
            Sections = sections
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(StructureFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateStructureFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _structuresClient.CreateAsync(model.SectionId, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create structure.");
            await PopulateStructureFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Structure created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _structuresClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Structure not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new StructureFormViewModel
        {
            Id = result.Data.Id,
            SectionId = result.Data.SectionId,
            Code = result.Data.Code,
            Name = result.Data.Name,
            StructureType = result.Data.StructureType,
            Levels = result.Data.Levels
        };

        await PopulateStructureFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(StructureFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            await PopulateStructureFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _structuresClient.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update structure.");
            await PopulateStructureFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Structure updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _structuresClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Structure not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _structuresClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate structure.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Structure deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateStructureFormOptionsAsync(StructureFormViewModel model, CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehouseOptionsAsync(null, cancellationToken);
        var sectionId = model.SectionId;

        var sectorId = Guid.Empty;
        if (sectionId != Guid.Empty)
        {
            var sectionResult = await _sectionsClient.GetByIdAsync(sectionId, cancellationToken);
            if (sectionResult.IsSuccess && sectionResult.Data is not null)
            {
                sectorId = sectionResult.Data.SectorId;
            }
        }

        var warehouseId = Guid.Empty;
        if (sectorId != Guid.Empty)
        {
            var sectorResult = await _sectorsClient.GetByIdAsync(sectorId, cancellationToken);
            if (sectorResult.IsSuccess && sectorResult.Data is not null)
            {
                warehouseId = sectorResult.Data.WarehouseId;
            }
        }

        if (warehouseId == Guid.Empty && warehouses.Count > 0)
        {
            warehouseId = Guid.Parse(warehouses[0].Value!);
        }

        var sectors = warehouseId == Guid.Empty
            ? Array.Empty<SelectListItem>()
            : await LoadSectorOptionsAsync(warehouseId, sectorId, cancellationToken);

        var sections = sectorId == Guid.Empty
            ? Array.Empty<SelectListItem>()
            : await LoadSectionOptionsAsync(sectorId, sectionId, cancellationToken);

        model.Warehouses = warehouses;
        model.Sectors = sectors;
        model.Sections = sections;
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

    private async Task<IReadOnlyList<SelectListItem>> LoadSectorOptionsAsync(Guid warehouseId, Guid? selectedSectorId, CancellationToken cancellationToken)
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

        return result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedSectorId.HasValue && item.Id == selectedSectorId.Value))
            .ToList();
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadSectionOptionsAsync(Guid sectorId, Guid? selectedSectionId, CancellationToken cancellationToken)
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

        return result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedSectionId.HasValue && item.Id == selectedSectionId.Value))
            .ToList();
    }
}
