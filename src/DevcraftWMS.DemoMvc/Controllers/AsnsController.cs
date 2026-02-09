using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.ViewModels.Asns;
using DevcraftWMS.DemoMvc.ViewModels.Products;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Uoms;
using DevcraftWMS.DemoMvc.ViewModels.Warehouses;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class AsnsController : Controller
{
    private readonly AsnsApiClient _asnsClient;
    private readonly WarehousesApiClient _warehousesClient;
    private readonly ProductsApiClient _productsClient;
    private readonly UomsApiClient _uomsClient;
    private readonly InboundOrdersApiClient _inboundOrdersClient;

    public AsnsController(
        AsnsApiClient asnsClient,
        WarehousesApiClient warehousesClient,
        ProductsApiClient productsClient,
        UomsApiClient uomsClient,
        InboundOrdersApiClient inboundOrdersClient)
    {
        _asnsClient = asnsClient;
        _warehousesClient = warehousesClient;
        _productsClient = productsClient;
        _uomsClient = uomsClient;
        _inboundOrdersClient = inboundOrdersClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] AsnQuery query, CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            TempData["Warning"] = "Select a customer to load ASNs.";
            return View(new AsnListPageViewModel
            {
                Query = query,
                Warehouses = Array.Empty<SelectListItem>()
            });
        }

        var (warehouses, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(query.WarehouseId, cancellationToken);
        if (!string.IsNullOrWhiteSpace(warehouseError))
        {
            TempData["Error"] = warehouseError;
        }

        var result = await _asnsClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load ASNs.";
            return View(new AsnListPageViewModel
            {
                Query = query,
                Warehouses = warehouses
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Asns",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["AsnNumber"] = query.AsnNumber,
                ["SupplierName"] = query.SupplierName,
                ["DocumentNumber"] = query.DocumentNumber,
                ["Status"] = query.Status?.ToString(),
                ["ExpectedFrom"] = query.ExpectedFrom?.ToString("yyyy-MM-dd"),
                ["ExpectedTo"] = query.ExpectedTo?.ToString("yyyy-MM-dd"),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new AsnListPageViewModel
        {
            Query = query,
            Items = result.Data.Items,
            Pagination = pagination,
            Warehouses = warehouses
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        if (!HasCustomerContext())
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "Select a customer first",
                Message = "ASNs require an active customer context. Select a customer to continue.",
                PrimaryActionText = "Go to customers",
                PrimaryActionUrl = Url.Action("Index", "Customers") ?? "#",
                SecondaryActionText = "Back to ASNs",
                SecondaryActionUrl = Url.Action("Index", "Asns") ?? "#",
                IconClass = "bi bi-person-check"
            };
            return View("DependencyPrompt", prompt);
        }

        var (warehouses, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(null, cancellationToken);
        if (!string.IsNullOrWhiteSpace(warehouseError))
        {
            TempData["Error"] = warehouseError;
        }

        if (warehouses.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No warehouse found",
                Message = "ASNs require a warehouse. Do you want to create a warehouse now?",
                PrimaryActionText = "Create warehouse",
                PrimaryActionUrl = Url.Action("Create", "Warehouses") ?? "#",
                SecondaryActionText = "Back to ASNs",
                SecondaryActionUrl = Url.Action("Index", "Asns") ?? "#",
                IconClass = "bi bi-box-seam"
            };
            return View("DependencyPrompt", prompt);
        }

        var model = new AsnFormViewModel
        {
            Warehouses = warehouses
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AsnFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Warehouses = (await LoadWarehouseOptionsWithErrorAsync(model.WarehouseId, cancellationToken)).Options;
            return View(model);
        }

        var result = await _asnsClient.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create ASN.");
            model.Warehouses = (await LoadWarehouseOptionsWithErrorAsync(model.WarehouseId, cancellationToken)).Options;
            return View(model);
        }

        TempData["Success"] = "ASN created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _asnsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "ASN not found.";
            return RedirectToAction(nameof(Index));
        }

        var (warehouses, warehouseError) = await LoadWarehouseOptionsWithErrorAsync(result.Data.WarehouseId, cancellationToken);
        if (!string.IsNullOrWhiteSpace(warehouseError))
        {
            TempData["Error"] = warehouseError;
        }

        var model = new AsnFormViewModel
        {
            Id = result.Data.Id,
            WarehouseId = result.Data.WarehouseId,
            AsnNumber = result.Data.AsnNumber,
            DocumentNumber = result.Data.DocumentNumber,
            SupplierName = result.Data.SupplierName,
            ExpectedArrivalDate = result.Data.ExpectedArrivalDate,
            Notes = result.Data.Notes,
            Warehouses = warehouses
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AsnFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Warehouses = (await LoadWarehouseOptionsWithErrorAsync(model.WarehouseId, cancellationToken)).Options;
            return View(model);
        }

        var result = await _asnsClient.UpdateAsync(id, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to update ASN.");
            model.Warehouses = (await LoadWarehouseOptionsWithErrorAsync(model.WarehouseId, cancellationToken)).Options;
            return View(model);
        }

        TempData["Success"] = "ASN updated successfully.";
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

        var attachmentsResult = await _asnsClient.ListAttachmentsAsync(id, cancellationToken);
        if (!attachmentsResult.IsSuccess)
        {
            TempData["Warning"] = attachmentsResult.Error ?? "Unable to load attachments.";
        }

        var itemsResult = await _asnsClient.ListItemsAsync(id, cancellationToken);
        if (!itemsResult.IsSuccess)
        {
            TempData["Warning"] = itemsResult.Error ?? "Unable to load items.";
        }

        var statusEventsResult = await _asnsClient.ListStatusEventsAsync(id, cancellationToken);
        if (!statusEventsResult.IsSuccess)
        {
            TempData["Warning"] = statusEventsResult.Error ?? "Unable to load status history.";
        }

        var products = await LoadProductOptionsAsync(Guid.Empty, cancellationToken);
        var uoms = await LoadUomOptionsAsync(Guid.Empty, cancellationToken);

        return View(new AsnDetailsPageViewModel
        {
            Asn = result.Data,
            Attachments = attachmentsResult.Data ?? Array.Empty<AsnAttachmentViewModel>(),
            Items = itemsResult.Data ?? Array.Empty<AsnItemViewModel>(),
            StatusEvents = statusEventsResult.Data ?? Array.Empty<AsnStatusEventViewModel>(),
            NewItem = new AsnItemCreateViewModel
            {
                Products = products,
                Uoms = uoms
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAttachment(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            TempData["Error"] = "Attachment file is required.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _asnsClient.UploadAttachmentAsync(id, file, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to upload attachment.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Attachment uploaded successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId, CancellationToken cancellationToken)
    {
        var result = await _asnsClient.DownloadAttachmentAsync(id, attachmentId, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to download attachment.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpGet]
    public async Task<IActionResult> PreviewAttachment(Guid id, Guid attachmentId, CancellationToken cancellationToken)
    {
        var result = await _asnsClient.DownloadAttachmentAsync(id, attachmentId, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to preview attachment.";
            return RedirectToAction(nameof(Details), new { id });
        }

        Response.Headers["Content-Disposition"] = $"inline; filename=\"{result.FileName}\"";
        return File(result.Content, result.ContentType);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(Guid id, [Bind(Prefix = "NewItem")] AsnItemCreateViewModel model, CancellationToken cancellationToken)
    {
        var productId = model.ProductId ?? TryGetGuidFromForm("NewItem.ProductId") ?? TryGetGuidFromForm("ProductId");
        var uomId = model.UomId ?? TryGetGuidFromForm("NewItem.UomId") ?? TryGetGuidFromForm("UomId");
        var quantity = model.Quantity ?? TryGetDecimalFromForm("NewItem.Quantity") ?? TryGetDecimalFromForm("Quantity");

        if (!ModelState.IsValid || !productId.HasValue || !uomId.HasValue || !quantity.HasValue)
        {
            TempData["Error"] = "Product, UoM, and quantity are required.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var payload = new
        {
            productId = productId.Value,
            uomId = uomId.Value,
            quantity = quantity.Value,
            lotCode = model.LotCode,
            expirationDate = model.ExpirationDate
        };

        var result = await _asnsClient.AddItemAsync(id, payload, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to add ASN item.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Item added successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(Guid id, string? notes, CancellationToken cancellationToken)
    {
        var result = await _asnsClient.SubmitAsync(id, notes, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to submit ASN.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "ASN submitted successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id, string? notes, CancellationToken cancellationToken)
    {
        var result = await _asnsClient.ApproveAsync(id, notes, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to approve ASN.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "ASN approved successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Convert(Guid id, string? notes, CancellationToken cancellationToken)
    {
        var result = await _inboundOrdersClient.ConvertFromAsnAsync(id, notes, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to convert ASN to inbound order.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (result.Data is null)
        {
            TempData["Warning"] = "Inbound order created, but details are unavailable.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Inbound order created from ASN.";
        return RedirectToAction("Details", "InboundOrders", new { id = result.Data.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string? notes, CancellationToken cancellationToken)
    {
        var result = await _asnsClient.CancelAsync(id, notes, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to cancel ASN.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "ASN canceled.";
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

        options.Insert(0, new SelectListItem("Selecione...", string.Empty, !selectedId.HasValue || selectedId == Guid.Empty));
        return (options, null);
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadProductOptionsAsync(Guid selectedId, CancellationToken cancellationToken)
    {
        var result = await _productsClient.ListAsync(
            new ProductQuery(PageNumber: 1, PageSize: 100, OrderBy: "Name", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), item.Id == selectedId))
            .ToList();

        options.Insert(0, new SelectListItem("Selecione...", string.Empty, selectedId == Guid.Empty));
        return options;
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadUomOptionsAsync(Guid selectedId, CancellationToken cancellationToken)
    {
        var result = await _uomsClient.ListAsync(
            new UomQuery(PageNumber: 1, PageSize: 100, OrderBy: "Code", OrderDir: "asc", IncludeInactive: false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        var options = result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), item.Id == selectedId))
            .ToList();

        options.Insert(0, new SelectListItem("Selecione...", string.Empty, selectedId == Guid.Empty));
        return options;
    }

    private bool HasCustomerContext()
    {
        var customerId = HttpContext.Session.GetStringValue(SessionKeys.CustomerId);
        return !string.IsNullOrWhiteSpace(customerId);
    }

    private Guid? TryGetGuidFromForm(string key)
    {
        if (!Request.HasFormContentType)
        {
            return null;
        }

        var value = Request.Form[key].ToString();
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }

    private decimal? TryGetDecimalFromForm(string key)
    {
        if (!Request.HasFormContentType)
        {
            return null;
        }

        var value = Request.Form[key].ToString();
        return decimal.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out var parsed)
            ? parsed
            : null;
    }
}
