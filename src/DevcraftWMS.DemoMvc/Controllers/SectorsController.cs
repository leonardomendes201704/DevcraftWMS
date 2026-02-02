using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Sectors;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class SectorsController : Controller
{
    private readonly SectorsApiClient _sectorsClient;
    private readonly WarehousesApiClient _warehousesClient;

    public SectorsController(SectorsApiClient sectorsClient, WarehousesApiClient warehousesClient)
    {
        _sectorsClient = sectorsClient;
        _warehousesClient = warehousesClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] SectorQuery query, CancellationToken cancellationToken)
    {
        var warehouseOptions = await LoadWarehouseOptionsAsync(query.WarehouseId, cancellationToken);
        if (warehouseOptions.Count == 0)
        {
            TempData["Warning"] = "Create a warehouse before managing sectors.";
            return View(new SectorListPageViewModel
            {
                Warehouses = warehouseOptions,
                Query = query
            });
        }

        var selectedWarehouseId = query.WarehouseId == Guid.Empty
            ? Guid.Parse(warehouseOptions[0].Value!)
            : query.WarehouseId;

        var normalizedQuery = query with { WarehouseId = selectedWarehouseId };
        var result = await _sectorsClient.ListAsync(normalizedQuery, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load sectors.";
            return View(new SectorListPageViewModel
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
            Controller = "Sectors",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = normalizedQuery.WarehouseId.ToString(),
                ["OrderBy"] = normalizedQuery.OrderBy,
                ["OrderDir"] = normalizedQuery.OrderDir,
                ["Code"] = normalizedQuery.Code,
                ["Name"] = normalizedQuery.Name,
                ["SectorType"] = normalizedQuery.SectorType?.ToString(),
                ["IsActive"] = normalizedQuery.IsActive?.ToString(),
                ["IncludeInactive"] = normalizedQuery.IncludeInactive.ToString(),
                ["PageSize"] = normalizedQuery.PageSize.ToString()
            }
        };

        var model = new SectorListPageViewModel
        {
            Items = result.Data.Items,
            Query = normalizedQuery,
            Pagination = pagination,
            Warehouses = warehouseOptions
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sectorsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Sector not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehouseOptionsAsync(null, cancellationToken);
        if (warehouses.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No warehouse found",
                Message = "Sectors depend on a warehouse. Do you want to create a warehouse now?",
                PrimaryActionText = "Create warehouse",
                PrimaryActionUrl = Url.Action("Create", "Warehouses") ?? "#",
                SecondaryActionText = "Back to sectors",
                SecondaryActionUrl = Url.Action("Index", "Sectors") ?? "#",
                IconClass = "bi bi-box-seam"
            };
            return View("DependencyPrompt", prompt);
        }

        var model = new SectorFormViewModel
        {
            WarehouseId = Guid.Parse(warehouses[0].Value!),
            Warehouses = warehouses
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SectorFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Warehouses = await LoadWarehouseOptionsAsync(model.WarehouseId, cancellationToken);
            return View(model);
        }

        var result = await _sectorsClient.CreateAsync(model.WarehouseId, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create sector.");
            model.Warehouses = await LoadWarehouseOptionsAsync(model.WarehouseId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Sector created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sectorsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Sector not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new SectorFormViewModel
        {
            Id = result.Data.Id,
            WarehouseId = result.Data.WarehouseId,
            Code = result.Data.Code,
            Name = result.Data.Name,
            Description = result.Data.Description,
            SectorType = result.Data.SectorType,
            Warehouses = await LoadWarehouseOptionsAsync(result.Data.WarehouseId, cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SectorFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            model.Warehouses = await LoadWarehouseOptionsAsync(model.WarehouseId, cancellationToken);
            return View(model);
        }

        var result = await _sectorsClient.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update sector.");
            model.Warehouses = await LoadWarehouseOptionsAsync(model.WarehouseId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Sector updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sectorsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Sector not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sectorsClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate sector.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Sector deactivated successfully.";
        return RedirectToAction(nameof(Index));
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
}
