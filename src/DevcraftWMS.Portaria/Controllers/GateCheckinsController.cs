using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.Portaria.ApiClients;
using DevcraftWMS.Portaria.ViewModels.GateCheckins;
using DevcraftWMS.Portaria.ViewModels.InboundOrders;
using DevcraftWMS.Portaria.ViewModels.Shared;

namespace DevcraftWMS.Portaria.Controllers;

public sealed class GateCheckinsController : Controller
{
    private readonly GateCheckinsApiClient _gateCheckinsApiClient;
    private readonly InboundOrdersApiClient _inboundOrdersApiClient;
    private readonly WarehousesApiClient _warehousesApiClient;

    public GateCheckinsController(
        GateCheckinsApiClient gateCheckinsApiClient,
        InboundOrdersApiClient inboundOrdersApiClient,
        WarehousesApiClient warehousesApiClient)
    {
        _gateCheckinsApiClient = gateCheckinsApiClient;
        _inboundOrdersApiClient = inboundOrdersApiClient;
        _warehousesApiClient = warehousesApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var pendingQuery = new GateCheckinListQuery(
            Status: GateCheckinStatus.CheckedIn,
            PageNumber: 1,
            PageSize: 20,
            OrderBy: "ArrivalAtUtc",
            OrderDir: "desc");

        var queueQuery = new GateCheckinListQuery(
            Status: GateCheckinStatus.WaitingDock,
            PageNumber: 1,
            PageSize: 50,
            OrderBy: "ArrivalAtUtc",
            OrderDir: "asc");

        var pendingResult = await _gateCheckinsApiClient.ListAsync(pendingQuery, cancellationToken);
        var queueResult = await _gateCheckinsApiClient.ListAsync(queueQuery, cancellationToken);

        if (!pendingResult.IsSuccess)
        {
            TempData["Error"] = pendingResult.Error ?? "Unable to load check-ins.";
        }

        if (!queueResult.IsSuccess)
        {
            TempData["Error"] = queueResult.Error ?? "Unable to load the waiting dock queue.";
        }

        ViewData["Title"] = "Gate Check-ins";
        ViewData["Subtitle"] = "Register vehicles and manage the waiting dock queue.";
        ViewData["Breadcrumbs"] = BuildBreadcrumbs("Gate Check-ins", Url.Action(nameof(Index)) ?? "#");

        var model = new GateCheckinIndexViewModel
        {
            PendingCheckins = pendingResult.Data?.Items ?? Array.Empty<GateCheckinListItemDto>(),
            WaitingDockQueue = queueResult.Data?.Items ?? Array.Empty<GateCheckinListItemDto>()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "New Check-in";
        ViewData["Subtitle"] = "Capture inbound vehicle arrival details.";
        ViewData["Breadcrumbs"] = BuildBreadcrumbs("Gate Check-ins", Url.Action(nameof(Index)) ?? "#", "New Check-in");

        return View(new GateCheckinCreateViewModel
        {
            ArrivalAtUtc = DateTime.UtcNow,
            Warehouses = await LoadWarehouseOptionsAsync(null, cancellationToken)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(GateCheckinCreateViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "New Check-in";
        ViewData["Subtitle"] = "Capture inbound vehicle arrival details.";
        ViewData["Breadcrumbs"] = BuildBreadcrumbs("Gate Check-ins", Url.Action(nameof(Index)) ?? "#", "New Check-in");

        model.Warehouses = await LoadWarehouseOptionsAsync(model.WarehouseId, cancellationToken);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var inboundOrderId = await ResolveInboundOrderIdAsync(model.InboundOrderNumber, cancellationToken);
        if (!string.IsNullOrWhiteSpace(model.InboundOrderNumber) && inboundOrderId is null)
        {
            ModelState.AddModelError(nameof(model.InboundOrderNumber), "Inbound order number was not found.");
            return View(model);
        }

        if (inboundOrderId is null && string.IsNullOrWhiteSpace(model.DocumentNumber))
        {
            ModelState.AddModelError(nameof(model.DocumentNumber), "Document number is required when inbound order number is not provided.");
            return View(model);
        }

        if (inboundOrderId is null && string.IsNullOrWhiteSpace(model.DocumentNumber) == false && model.WarehouseId is null)
        {
            ModelState.AddModelError(nameof(model.WarehouseId), "Warehouse is required to create an emergency inbound order.");
            return View(model);
        }

        var request = new GateCheckinCreateRequest(
            inboundOrderId,
            string.IsNullOrWhiteSpace(model.DocumentNumber) ? null : model.DocumentNumber.Trim(),
            model.VehiclePlate.Trim(),
            model.DriverName.Trim(),
            string.IsNullOrWhiteSpace(model.CarrierName) ? null : model.CarrierName.Trim(),
            model.ArrivalAtUtc,
            string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim(),
            model.WarehouseId);

        var result = await _gateCheckinsApiClient.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create the check-in.");
            return View(model);
        }

        TempData["Success"] = "Check-in created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SendToQueue(Guid id, CancellationToken cancellationToken)
    {
        var detailResult = await _gateCheckinsApiClient.GetByIdAsync(id, cancellationToken);
        if (!detailResult.IsSuccess || detailResult.Data is null)
        {
            TempData["Error"] = detailResult.Error ?? "Unable to load the check-in.";
            return RedirectToAction(nameof(Index));
        }

        var updateRequest = new GateCheckinUpdateRequest(
            detailResult.Data.InboundOrderId,
            detailResult.Data.DocumentNumber,
            detailResult.Data.VehiclePlate,
            detailResult.Data.DriverName,
            detailResult.Data.CarrierName,
            detailResult.Data.ArrivalAtUtc,
            (int)GateCheckinStatus.WaitingDock,
            detailResult.Data.Notes);

        var updateResult = await _gateCheckinsApiClient.UpdateAsync(id, updateRequest, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            TempData["Error"] = updateResult.Error ?? "Unable to move the check-in to the waiting dock queue.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Check-in moved to the waiting dock queue.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> AssignDock(Guid id, CancellationToken cancellationToken)
    {
        var detailResult = await _gateCheckinsApiClient.GetByIdAsync(id, cancellationToken);
        if (!detailResult.IsSuccess || detailResult.Data is null)
        {
            TempData["Error"] = detailResult.Error ?? "Unable to load the check-in.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "Assign Dock";
        ViewData["Subtitle"] = "Assign a dock and move the vehicle to the dock status.";
        ViewData["Breadcrumbs"] = BuildBreadcrumbs("Gate Check-ins", Url.Action(nameof(Index)) ?? "#", "Assign Dock");

        var model = new GateCheckinAssignDockViewModel
        {
            Id = detailResult.Data.Id,
            DockCode = detailResult.Data.DockCode ?? string.Empty,
            VehiclePlate = detailResult.Data.VehiclePlate,
            InboundOrderNumber = detailResult.Data.InboundOrderNumber,
            ArrivalAtUtc = detailResult.Data.ArrivalAtUtc
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AssignDock(GateCheckinAssignDockViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Assign Dock";
        ViewData["Subtitle"] = "Assign a dock and move the vehicle to the dock status.";
        ViewData["Breadcrumbs"] = BuildBreadcrumbs("Gate Check-ins", Url.Action(nameof(Index)) ?? "#", "Assign Dock");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _gateCheckinsApiClient.AssignDockAsync(model.Id, model.DockCode.Trim(), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to assign the dock.");
            return View(model);
        }

        TempData["Success"] = "Dock assigned successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var detailResult = await _gateCheckinsApiClient.GetByIdAsync(id, cancellationToken);
        if (!detailResult.IsSuccess || detailResult.Data is null)
        {
            TempData["Error"] = detailResult.Error ?? "Unable to load the check-in.";
            return RedirectToAction(nameof(Index));
        }

        var updateRequest = new GateCheckinUpdateRequest(
            detailResult.Data.InboundOrderId,
            detailResult.Data.DocumentNumber,
            detailResult.Data.VehiclePlate,
            detailResult.Data.DriverName,
            detailResult.Data.CarrierName,
            detailResult.Data.ArrivalAtUtc,
            (int)GateCheckinStatus.Canceled,
            detailResult.Data.Notes);

        var updateResult = await _gateCheckinsApiClient.UpdateAsync(id, updateRequest, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            TempData["Error"] = updateResult.Error ?? "Unable to cancel the check-in.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Check-in canceled.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Guid?> ResolveInboundOrderIdAsync(string? inboundOrderNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(inboundOrderNumber))
        {
            return null;
        }

        var query = new InboundOrderListQuery(
            PageNumber: 1,
            PageSize: 5,
            OrderBy: "CreatedAtUtc",
            OrderDir: "desc",
            OrderNumber: inboundOrderNumber.Trim());

        var result = await _inboundOrdersApiClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return null;
        }

        var match = result.Data.Items.FirstOrDefault(item =>
            string.Equals(item.OrderNumber, inboundOrderNumber.Trim(), StringComparison.OrdinalIgnoreCase));

        return match?.Id;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadWarehouseOptionsAsync(Guid? selectedId, CancellationToken cancellationToken)
    {
        var result = await _warehousesApiClient.ListAsync(new WarehouseListQuery(PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc"), cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load warehouses.";
            return new List<SelectListItem>
            {
                new("Select...", string.Empty, selectedId is null)
            };
        }

        var options = result.Data.Items
            .Select(wh => new SelectListItem($"{wh.Code} - {wh.Name}", wh.Id.ToString(), selectedId == wh.Id))
            .ToList();
        options.Insert(0, new SelectListItem("Select...", string.Empty, selectedId is null));
        return options;
    }

    private static IReadOnlyList<BreadcrumbItem> BuildBreadcrumbs(string rootLabel, string rootUrl, string? currentLabel = null)
    {
        if (string.IsNullOrWhiteSpace(currentLabel))
        {
            return new List<BreadcrumbItem>
            {
                new()
                {
                    Title = rootLabel,
                    Url = rootUrl,
                    IsActive = true
                }
            };
        }

        return new List<BreadcrumbItem>
        {
            new()
            {
                Title = rootLabel,
                Url = rootUrl,
                IsActive = false
            },
            new()
            {
                Title = currentLabel,
                Url = "#",
                IsActive = true
            }
        };
    }
}
