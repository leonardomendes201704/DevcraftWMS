using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.Receipts;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.UnitLoads;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class UnitLoadsController : Controller
{
    private const string TempDataLabelKey = "UnitLoadLabel";

    private readonly UnitLoadsApiClient _unitLoadsClient;
    private readonly ReceiptsApiClient _receiptsClient;
    private readonly WarehousesApiClient _warehousesClient;

    public UnitLoadsController(
        UnitLoadsApiClient unitLoadsClient,
        ReceiptsApiClient receiptsClient,
        WarehousesApiClient warehousesClient)
    {
        _unitLoadsClient = unitLoadsClient;
        _receiptsClient = receiptsClient;
        _warehousesClient = warehousesClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] UnitLoadQuery query, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load unit loads.";
            return View(new UnitLoadListPageViewModel
            {
                Query = query,
                Warehouses = Array.Empty<SelectListItem>(),
                Items = Array.Empty<UnitLoadListItemViewModel>()
            });
        }

        var (warehouseOptions, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(query.WarehouseId, cancellationToken);
        if (!string.IsNullOrWhiteSpace(warehouseError))
        {
            TempData["Error"] = warehouseError;
        }

        var normalizedQuery = query with
        {
            WarehouseId = query.WarehouseId.HasValue && query.WarehouseId.Value == Guid.Empty ? null : query.WarehouseId,
            ReceiptId = query.ReceiptId.HasValue && query.ReceiptId.Value == Guid.Empty ? null : query.ReceiptId
        };

        var result = await _unitLoadsClient.ListAsync(normalizedQuery, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load unit loads.";
            return View(new UnitLoadListPageViewModel
            {
                Query = normalizedQuery,
                Warehouses = warehouseOptions
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "UnitLoads",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = normalizedQuery.WarehouseId?.ToString(),
                ["ReceiptId"] = normalizedQuery.ReceiptId?.ToString(),
                ["Sscc"] = normalizedQuery.Sscc,
                ["Status"] = normalizedQuery.Status?.ToString(),
                ["IsActive"] = normalizedQuery.IsActive?.ToString(),
                ["IncludeInactive"] = normalizedQuery.IncludeInactive.ToString(),
                ["OrderBy"] = normalizedQuery.OrderBy,
                ["OrderDir"] = normalizedQuery.OrderDir,
                ["PageSize"] = normalizedQuery.PageSize.ToString()
            }
        };

        var model = new UnitLoadListPageViewModel
        {
            Items = result.Data.Items,
            Pagination = pagination,
            Query = normalizedQuery,
            Warehouses = warehouseOptions
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? receiptId, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "Select a customer first",
                Message = "Unit loads require an active customer context. Select a customer to continue.",
                PrimaryActionText = "Go to customers",
                PrimaryActionUrl = Url.Action("Index", "Customers") ?? "#",
                SecondaryActionText = "Back to unit loads",
                SecondaryActionUrl = Url.Action("Index", "UnitLoads") ?? "#",
                IconClass = "bi bi-person-check"
            };
            return View("DependencyPrompt", prompt);
        }

        var receiptOptions = await LoadReceiptOptionsAsync(receiptId, cancellationToken);
        if (receiptOptions.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No receipt found",
                Message = "Create a receipt before generating unit loads.",
                PrimaryActionText = "Go to receipts",
                PrimaryActionUrl = Url.Action("Index", "Receipts") ?? "#",
                SecondaryActionText = "Back to unit loads",
                SecondaryActionUrl = Url.Action("Index", "UnitLoads") ?? "#",
                IconClass = "bi bi-receipt"
            };
            return View("DependencyPrompt", prompt);
        }

        var model = new UnitLoadFormViewModel
        {
            ReceiptId = receiptId ?? Guid.Empty,
            Receipts = receiptOptions
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UnitLoadFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Receipts = await LoadReceiptOptionsAsync(model.ReceiptId, cancellationToken);
            return View(model);
        }

        var result = await _unitLoadsClient.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create unit load.");
            model.Receipts = await LoadReceiptOptionsAsync(model.ReceiptId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Unit load created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _unitLoadsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unit load not found.";
            return RedirectToAction(nameof(Index));
        }

        var label = ReadLabelFromTempData();
        var model = new UnitLoadDetailsPageViewModel
        {
            UnitLoad = result.Data,
            Label = label
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Print(Guid id, CancellationToken cancellationToken)
    {
        var result = await _unitLoadsClient.PrintLabelAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to print label.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Label generated successfully.";
        TempData[TempDataLabelKey] = JsonSerializer.Serialize(result.Data);
        return RedirectToAction(nameof(Details), new { id });
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

    private async Task<IReadOnlyList<SelectListItem>> LoadReceiptOptionsAsync(Guid? selectedId, CancellationToken cancellationToken)
    {
        var query = new ReceiptQuery(
            PageNumber: 1,
            PageSize: 100,
            OrderBy: "CreatedAtUtc",
            OrderDir: "desc",
            WarehouseId: null,
            ReceiptNumber: null,
            DocumentNumber: null,
            SupplierName: null,
            Status: null,
            ReceivedFrom: null,
            ReceivedTo: null,
            IsActive: true,
            IncludeInactive: false);

        var result = await _receiptsClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return BuildEmptyOptions();
        }

        var options = result.Data.Items
            .Select(item =>
            {
                var label = string.IsNullOrWhiteSpace(item.DocumentNumber)
                    ? item.ReceiptNumber
                    : $"{item.ReceiptNumber} - {item.DocumentNumber}";
                return new SelectListItem(label, item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value);
            })
            .ToList();

        return AddSelectPrompt(options, selectedId.HasValue);
    }

    private UnitLoadLabelViewModel? ReadLabelFromTempData()
    {
        if (TempData.TryGetValue(TempDataLabelKey, out var value) && value is string json && !string.IsNullOrWhiteSpace(json))
        {
            return JsonSerializer.Deserialize<UnitLoadLabelViewModel>(json);
        }

        return null;
    }

    private bool HasCustomerContext()
        => !string.IsNullOrWhiteSpace(HttpContext.Session.GetStringValue(SessionKeys.CustomerId));

    private static IReadOnlyList<SelectListItem> BuildEmptyOptions()
        => new List<SelectListItem> { new("Select...", string.Empty, true) };

    private static IReadOnlyList<SelectListItem> AddSelectPrompt(IReadOnlyList<SelectListItem> items, bool hasSelection)
    {
        var list = items.ToList();
        list.Insert(0, new SelectListItem("Select...", string.Empty, !hasSelection));
        return list;
    }
}
