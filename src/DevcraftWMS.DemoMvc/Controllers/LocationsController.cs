using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Locations;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using DevcraftWMS.DemoMvc.ViewModels.Sectors;
using DevcraftWMS.DemoMvc.ViewModels.Sections;
using DevcraftWMS.DemoMvc.ViewModels.Structures;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class LocationsController : Controller
{
    private readonly LocationsApiClient _locationsClient;
    private readonly WarehousesApiClient _warehousesClient;
    private readonly SectorsApiClient _sectorsClient;
    private readonly SectionsApiClient _sectionsClient;
    private readonly StructuresApiClient _structuresClient;
    private readonly ZonesApiClient _zonesClient;

    public LocationsController(
        LocationsApiClient locationsClient,
        WarehousesApiClient warehousesClient,
        SectorsApiClient sectorsClient,
        SectionsApiClient sectionsClient,
        StructuresApiClient structuresClient,
        ZonesApiClient zonesClient)
    {
        _locationsClient = locationsClient;
        _warehousesClient = warehousesClient;
        _sectorsClient = sectorsClient;
        _sectionsClient = sectionsClient;
        _structuresClient = structuresClient;
        _zonesClient = zonesClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] LocationQuery query, CancellationToken cancellationToken)
    {
        var warehouseOptions = await LoadWarehouseOptionsAsync(query.WarehouseId, cancellationToken);
        if (warehouseOptions.Count == 0)
        {
            TempData["Warning"] = "Create a warehouse before managing locations.";
            return View(new LocationListPageViewModel
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
            TempData["Warning"] = "Create a sector before managing locations.";
            return View(new LocationListPageViewModel
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
            TempData["Warning"] = "Create a section before managing locations.";
            return View(new LocationListPageViewModel
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

        var structureOptions = await LoadStructureOptionsAsync(selectedSectionId, query.StructureId, cancellationToken);
        if (structureOptions.Count == 0)
        {
            TempData["Warning"] = "Create a structure before managing locations.";
            return View(new LocationListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Sections = sectionOptions,
                Structures = structureOptions,
                Query = query with { WarehouseId = selectedWarehouseId, SectorId = selectedSectorId, SectionId = selectedSectionId }
            });
        }

        var selectedStructureId = query.StructureId == Guid.Empty
            ? Guid.Parse(structureOptions[0].Value!)
            : query.StructureId;

        var normalizedQuery = query with
        {
            WarehouseId = selectedWarehouseId,
            SectorId = selectedSectorId,
            SectionId = selectedSectionId,
            StructureId = selectedStructureId
        };

        var zoneOptions = await LoadZoneOptionsAsync(selectedWarehouseId, normalizedQuery.ZoneId, cancellationToken);

        var result = await _locationsClient.ListAsync(normalizedQuery, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load locations.";
            return View(new LocationListPageViewModel
            {
                Warehouses = warehouseOptions,
                Sectors = sectorOptions,
                Sections = sectionOptions,
                Structures = structureOptions,
                Zones = zoneOptions,
                Query = normalizedQuery
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Locations",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = normalizedQuery.WarehouseId.ToString(),
                ["SectorId"] = normalizedQuery.SectorId.ToString(),
                ["SectionId"] = normalizedQuery.SectionId.ToString(),
                ["StructureId"] = normalizedQuery.StructureId.ToString(),
                ["ZoneId"] = normalizedQuery.ZoneId?.ToString(),
                ["OrderBy"] = normalizedQuery.OrderBy,
                ["OrderDir"] = normalizedQuery.OrderDir,
                ["Code"] = normalizedQuery.Code,
                ["Barcode"] = normalizedQuery.Barcode,
                ["IsActive"] = normalizedQuery.IsActive?.ToString(),
                ["IncludeInactive"] = normalizedQuery.IncludeInactive.ToString(),
                ["PageSize"] = normalizedQuery.PageSize.ToString()
            }
        };

        var model = new LocationListPageViewModel
        {
            Items = result.Data.Items,
            Query = normalizedQuery,
            Pagination = pagination,
            Warehouses = warehouseOptions,
            Sectors = sectorOptions,
            Sections = sectionOptions,
            Structures = structureOptions,
            Zones = zoneOptions
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Location not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? warehouseId, Guid? sectorId, Guid? sectionId, Guid? structureId, CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehouseOptionsAsync(null, cancellationToken);
        if (warehouses.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No warehouse found",
                Message = "Locations depend on a warehouse, sector, section, and structure. Do you want to create a warehouse now?",
                PrimaryActionText = "Create warehouse",
                PrimaryActionUrl = Url.Action("Create", "Warehouses") ?? "#",
                SecondaryActionText = "Back to locations",
                SecondaryActionUrl = Url.Action("Index", "Locations") ?? "#",
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
                Message = "Locations depend on a sector, section, and structure. Do you want to create a sector now?",
                PrimaryActionText = "Create sector",
                PrimaryActionUrl = Url.Action("Create", "Sectors", new { warehouseId = selectedWarehouseId }) ?? "#",
                SecondaryActionText = "Back to locations",
                SecondaryActionUrl = Url.Action("Index", "Locations", new { warehouseId = selectedWarehouseId }) ?? "#",
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
                Message = "Locations depend on a section and structure. Do you want to create a section now?",
                PrimaryActionText = "Create section",
                PrimaryActionUrl = Url.Action("Create", "Sections", new { warehouseId = selectedWarehouseId, sectorId = selectedSectorId }) ?? "#",
                SecondaryActionText = "Back to locations",
                SecondaryActionUrl = Url.Action("Index", "Locations", new { warehouseId = selectedWarehouseId, sectorId = selectedSectorId }) ?? "#",
                IconClass = "bi bi-layout-text-sidebar"
            };
            return View("DependencyPrompt", prompt);
        }

        var selectedSectionId = sectionId.HasValue && sections.Any(s => s.Value == sectionId.Value.ToString())
            ? sectionId.Value
            : Guid.Parse(sections[0].Value!);

        var structures = await LoadStructureOptionsAsync(selectedSectionId, structureId, cancellationToken);
        if (structures.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No structure found",
                Message = "Locations depend on a structure. Do you want to create a structure now?",
                PrimaryActionText = "Create structure",
                PrimaryActionUrl = Url.Action("Create", "Structures", new { warehouseId = selectedWarehouseId, sectorId = selectedSectorId, sectionId = selectedSectionId }) ?? "#",
                SecondaryActionText = "Back to locations",
                SecondaryActionUrl = Url.Action("Index", "Locations", new { warehouseId = selectedWarehouseId, sectorId = selectedSectorId, sectionId = selectedSectionId }) ?? "#",
                IconClass = "bi bi-stack"
            };
            return View("DependencyPrompt", prompt);
        }

        var selectedStructureId = structureId.HasValue && structures.Any(s => s.Value == structureId.Value.ToString())
            ? structureId.Value
            : Guid.Parse(structures[0].Value!);

        var model = new LocationFormViewModel
        {
            StructureId = selectedStructureId,
            Warehouses = warehouses,
            Sectors = sectors,
            Sections = sections,
            Structures = structures,
            Zones = await LoadZoneOptionsAsync(selectedWarehouseId, null, cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LocationFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLocationFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _locationsClient.CreateAsync(model.StructureId, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create location.");
            await PopulateLocationFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Location created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Location not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new LocationFormViewModel
        {
            Id = result.Data.Id,
            StructureId = result.Data.StructureId,
            ZoneId = result.Data.ZoneId,
            Code = result.Data.Code,
            Barcode = result.Data.Barcode,
            Level = result.Data.Level,
            Row = result.Data.Row,
            Column = result.Data.Column,
            MaxWeightKg = result.Data.MaxWeightKg,
            MaxVolumeM3 = result.Data.MaxVolumeM3,
            AllowLotTracking = result.Data.AllowLotTracking,
            AllowExpiryTracking = result.Data.AllowExpiryTracking
        };

        await PopulateLocationFormOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LocationFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            await PopulateLocationFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _locationsClient.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update location.");
            await PopulateLocationFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Location updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Location not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationsClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate location.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Location deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLocationFormOptionsAsync(LocationFormViewModel model, CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehouseOptionsAsync(null, cancellationToken);
        var structureId = model.StructureId;

        var sectionId = Guid.Empty;
        if (structureId != Guid.Empty)
        {
            var structureResult = await _structuresClient.GetByIdAsync(structureId, cancellationToken);
            if (structureResult.IsSuccess && structureResult.Data is not null)
            {
                sectionId = structureResult.Data.SectionId;
            }
        }

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

        var structures = sectionId == Guid.Empty
            ? Array.Empty<SelectListItem>()
            : await LoadStructureOptionsAsync(sectionId, structureId, cancellationToken);

        model.Warehouses = warehouses;
        model.Sectors = sectors;
        model.Sections = sections;
        model.Structures = structures;
        model.Zones = warehouseId == Guid.Empty
            ? Array.Empty<SelectListItem>()
            : await LoadZoneOptionsAsync(warehouseId, model.ZoneId, cancellationToken);
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

    private async Task<IReadOnlyList<SelectListItem>> LoadStructureOptionsAsync(Guid sectionId, Guid? selectedStructureId, CancellationToken cancellationToken)
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

        return result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedStructureId.HasValue && item.Id == selectedStructureId.Value))
            .ToList();
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadZoneOptionsAsync(Guid warehouseId, Guid? selectedZoneId, CancellationToken cancellationToken)
    {
        var result = await _zonesClient.ListAsync(
            new ViewModels.Zones.ZoneQuery(
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
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedZoneId.HasValue && item.Id == selectedZoneId.Value))
            .ToList();

        items.Insert(0, new SelectListItem("Select...", string.Empty, !selectedZoneId.HasValue || selectedZoneId == Guid.Empty));
        return items;
    }
}
