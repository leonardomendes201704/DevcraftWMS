using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class WarehousesController : Controller
{
    private readonly WarehousesApiClient _client;

    public WarehousesController(WarehousesApiClient client)
    {
        _client = client;
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

        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new WarehouseFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(WarehouseFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _client.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create warehouse.");
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

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(WarehouseFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            return View(model);
        }

        var result = await _client.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update warehouse.");
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
}
