using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Portal.ApiClients;
using DevcraftWMS.Portal.ViewModels.Asns;
using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.Controllers;

public sealed class AsnsController : Controller
{
    private readonly AsnsApiClient _asnsClient;
    private readonly WarehousesApiClient _warehousesClient;

    public AsnsController(AsnsApiClient asnsClient, WarehousesApiClient warehousesClient)
    {
        _asnsClient = asnsClient;
        _warehousesClient = warehousesClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] AsnListQuery query, CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehousesAsync(cancellationToken);
        var result = await _asnsClient.ListAsync(query, cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load ASNs.";
        }

        var model = new AsnListViewModel
        {
            Query = query,
            Items = result.Data?.Items ?? Array.Empty<AsnListItemDto>(),
            TotalCount = result.Data?.TotalCount ?? 0,
            Warehouses = warehouses
        };

        ViewData["Title"] = "ASNs";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehousesAsync(cancellationToken);
        if (warehouses.Count == 0)
        {
            TempData["Warning"] = "No warehouses available. Please contact backoffice to register a warehouse.";
        }

        var model = new AsnCreateViewModel
        {
            Warehouses = warehouses
        };

        ViewData["Title"] = "Create ASN";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AsnCreateViewModel model, CancellationToken cancellationToken)
    {
        var warehouses = await LoadWarehousesAsync(cancellationToken);
        model.Warehouses = warehouses;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!model.WarehouseId.HasValue || model.WarehouseId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(model.WarehouseId), "Warehouse is required.");
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.AsnNumber))
        {
            ModelState.AddModelError(nameof(model.AsnNumber), "ASN Number is required.");
            return View(model);
        }

        var request = new AsnCreateRequest(
            model.WarehouseId.Value,
            model.AsnNumber.Trim(),
            model.DocumentNumber?.Trim(),
            model.SupplierName?.Trim(),
            model.ExpectedArrivalDate,
            model.Notes?.Trim());

        var result = await _asnsClient.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create ASN.");
            return View(model);
        }

        TempData["Success"] = "ASN created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _asnsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "ASN not found.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = $"ASN {result.Data.AsnNumber}";
        return View(new AsnDetailViewModel { Asn = result.Data });
    }

    private async Task<IReadOnlyList<WarehouseOptionDto>> LoadWarehousesAsync(CancellationToken cancellationToken)
    {
        var result = await _warehousesClient.ListAsync(100, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load warehouses.";
            return Array.Empty<WarehouseOptionDto>();
        }

        return result.Data.Items;
    }
}
