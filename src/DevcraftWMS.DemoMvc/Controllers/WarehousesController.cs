using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using DevcraftWMS.DemoMvc.ViewModels.Locations;
using DevcraftWMS.DemoMvc.ViewModels.Sections;
using DevcraftWMS.DemoMvc.ViewModels.Sectors;
using DevcraftWMS.DemoMvc.ViewModels.Structures;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class WarehousesController : Controller
{
    private readonly WarehousesApiClient _client;
    private readonly CostCentersApiClient _costCentersClient;
    private readonly SectorsApiClient _sectorsClient;
    private readonly SectionsApiClient _sectionsClient;
    private readonly StructuresApiClient _structuresClient;
    private readonly LocationsApiClient _locationsClient;
    private readonly ILogger<WarehousesController> _logger;

    public WarehousesController(
        WarehousesApiClient client,
        CostCentersApiClient costCentersClient,
        SectorsApiClient sectorsClient,
        SectionsApiClient sectionsClient,
        StructuresApiClient structuresClient,
        LocationsApiClient locationsClient,
        ILogger<WarehousesController> logger)
    {
        _client = client;
        _costCentersClient = costCentersClient;
        _sectorsClient = sectorsClient;
        _sectionsClient = sectionsClient;
        _structuresClient = structuresClient;
        _locationsClient = locationsClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] WarehouseQuery query, CancellationToken cancellationToken)
    {
        var result = await _client.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load warehouses.";
            return View(new WarehouseListPageViewModel());
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Warehouses",
            Query = new Dictionary<string, string?>
            {
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["Search"] = query.Search,
                ["Code"] = query.Code,
                ["Name"] = query.Name,
                ["WarehouseType"] = query.WarehouseType?.ToString(),
                ["City"] = query.City,
                ["State"] = query.State,
                ["Country"] = query.Country,
                ["ExternalId"] = query.ExternalId,
                ["ErpCode"] = query.ErpCode,
                ["CostCenterCode"] = query.CostCenterCode,
                ["IsPrimary"] = query.IsPrimary?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        var model = new WarehouseListPageViewModel
        {
            Items = result.Data.Items,
            Query = query,
            Pagination = pagination
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Warehouse not found.";
            return RedirectToAction(nameof(Index));
        }

        var sectorsResult = await _sectorsClient.ListAsync(new SectorQuery(
            WarehouseId: id,
            PageNumber: 1,
            PageSize: 100,
            OrderBy: "Code",
            OrderDir: "asc",
            Code: null,
            Name: null,
            SectorType: null,
            IsActive: null,
            IncludeInactive: false), cancellationToken);

        var sectionsResult = await _sectionsClient.ListAsync(new SectionQuery(
            WarehouseId: id,
            SectorId: null,
            PageNumber: 1,
            PageSize: 100,
            OrderBy: "Code",
            OrderDir: "asc",
            Code: null,
            Name: null,
            IsActive: null,
            IncludeInactive: false), cancellationToken);

        var structuresResult = await _structuresClient.ListAsync(new StructureQuery(
            WarehouseId: id,
            SectorId: null,
            SectionId: null,
            PageNumber: 1,
            PageSize: 100,
            OrderBy: "Code",
            OrderDir: "asc",
            Code: null,
            Name: null,
            StructureType: null,
            IsActive: null,
            IncludeInactive: false), cancellationToken);

        var locationsResult = await _locationsClient.ListAsync(new LocationQuery(
            WarehouseId: id,
            SectorId: null,
            SectionId: null,
            StructureId: null,
            ZoneId: null,
            PageNumber: 1,
            PageSize: 100,
            OrderBy: "Code",
            OrderDir: "asc",
            Code: null,
            Barcode: null,
            IsActive: null,
            IncludeInactive: false), cancellationToken);

        if (!sectorsResult.IsSuccess)
        {
            TempData["Warning"] = sectorsResult.Error ?? "Failed to load sectors.";
        }

        if (!sectionsResult.IsSuccess)
        {
            TempData["Warning"] = sectionsResult.Error ?? "Failed to load sections.";
        }

        if (!structuresResult.IsSuccess)
        {
            TempData["Warning"] = structuresResult.Error ?? "Failed to load structures.";
        }

        if (!locationsResult.IsSuccess)
        {
            TempData["Warning"] = locationsResult.Error ?? "Failed to load locations.";
        }

        var model = new WarehouseDetailsPageViewModel
        {
            Warehouse = result.Data,
            Sectors = sectorsResult.Data?.Items ?? Array.Empty<SectorListItemViewModel>(),
            SectorsTotal = sectorsResult.Data?.TotalCount ?? 0,
            Sections = sectionsResult.Data?.Items ?? Array.Empty<SectionListItemViewModel>(),
            SectionsTotal = sectionsResult.Data?.TotalCount ?? 0,
            Structures = structuresResult.Data?.Items ?? Array.Empty<StructureListItemViewModel>(),
            StructuresTotal = structuresResult.Data?.TotalCount ?? 0,
            Locations = locationsResult.Data?.Items ?? Array.Empty<LocationListItemViewModel>(),
            LocationsTotal = locationsResult.Data?.TotalCount ?? 0
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new WarehouseFormViewModel
        {
            Code = "AUTO"
        };
        await LoadCostCentersAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(WarehouseFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            LogModelStateErrors("Create", model);
            await LoadCostCentersAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _client.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create warehouse.");
            LogModelStateErrors("Create.ApiFailure", model, result.Error);
            await LoadCostCentersAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Warehouse created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Warehouse not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new WarehouseFormViewModel
        {
            Id = result.Data.Id,
            Code = result.Data.Code,
            Name = result.Data.Name,
            ShortName = result.Data.ShortName,
            Description = result.Data.Description,
            WarehouseType = result.Data.WarehouseType,
            IsPrimary = result.Data.IsPrimary,
            IsPickingEnabled = result.Data.IsPickingEnabled,
            IsReceivingEnabled = result.Data.IsReceivingEnabled,
            IsShippingEnabled = result.Data.IsShippingEnabled,
            IsReturnsEnabled = result.Data.IsReturnsEnabled,
            ExternalId = result.Data.ExternalId,
            ErpCode = result.Data.ErpCode,
            CostCenterCode = result.Data.CostCenterCode,
            CostCenterName = result.Data.CostCenterName,
            CutoffTime = result.Data.CutoffTime,
            Timezone = result.Data.Timezone,
            Address = result.Data.Addresses.FirstOrDefault() is { } address ? new AddressInputViewModel
            {
                AddressLine1 = address.AddressLine1,
                AddressNumber = address.AddressNumber,
                AddressLine2 = address.AddressLine2,
                District = address.District,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                Latitude = address.Latitude,
                Longitude = address.Longitude
            } : new AddressInputViewModel(),
            Contact = result.Data.Contacts.FirstOrDefault() is { } contact ? new WarehouseContactViewModel
            {
                ContactName = contact.ContactName,
                ContactEmail = contact.ContactEmail,
                ContactPhone = contact.ContactPhone
            } : new WarehouseContactViewModel(),
            Capacity = result.Data.Capacities.FirstOrDefault() is { } capacity ? new WarehouseCapacityViewModel
            {
                LengthMeters = capacity.LengthMeters,
                WidthMeters = capacity.WidthMeters,
                HeightMeters = capacity.HeightMeters,
                TotalAreaM2 = capacity.TotalAreaM2,
                TotalCapacity = capacity.TotalCapacity,
                CapacityUnit = capacity.CapacityUnit,
                MaxWeightKg = capacity.MaxWeightKg,
                OperationalArea = capacity.OperationalArea
            } : new WarehouseCapacityViewModel()
        };

        await LoadCostCentersAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(WarehouseFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            LogModelStateErrors("Edit", model);
            await LoadCostCentersAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _client.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update warehouse.");
            LogModelStateErrors("Edit.ApiFailure", model, result.Error);
            await LoadCostCentersAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Warehouse updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Warehouse not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate warehouse.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Warehouse deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCostCentersAsync(WarehouseFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _costCentersClient.ListAsync(new ViewModels.CostCenters.CostCenterQuery(PageNumber: 1, PageSize: 200, OrderBy: "Code", OrderDir: "asc", IncludeInactive: false), cancellationToken);
        model.CostCenters = result.Data?.Items
            .Select(item => new WarehouseCostCenterOptionViewModel(item.Code, item.Name))
            .ToList()
            ?? new List<WarehouseCostCenterOptionViewModel>();
    }

    private void LogModelStateErrors(string context, WarehouseFormViewModel model, string? apiError = null)
    {
        var errors = ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => new { Field = entry.Key, error.ErrorMessage }))
            .ToList();

        if (errors.Count == 0)
        {
            return;
        }

        _logger.LogWarning(
            "Warehouse form validation failed ({Context}). Errors: {@Errors}. Code={Code} Name={Name} CostCenter={CostCenterCode} ApiError={ApiError}",
            context,
            errors,
            model.Code,
            model.Name,
            model.CostCenterCode,
            apiError);
    }
}
