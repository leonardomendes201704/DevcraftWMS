using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Lots;
using DevcraftWMS.DemoMvc.ViewModels.Products;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class LotsController : Controller
{
    private readonly LotsApiClient _lotsClient;
    private readonly ProductsApiClient _productsClient;

    public LotsController(LotsApiClient lotsClient, ProductsApiClient productsClient)
    {
        _lotsClient = lotsClient;
        _productsClient = productsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] LotQuery query, CancellationToken cancellationToken)
    {
        var productOptions = await LoadProductOptionsAsync(query.ProductId, cancellationToken);
        if (productOptions.Count == 0)
        {
            TempData["Warning"] = "Create a product before managing lots.";
            return View(new LotListPageViewModel
            {
                Products = productOptions,
                Query = query
            });
        }

        var selectedProductId = query.ProductId == Guid.Empty
            ? Guid.Parse(productOptions[0].Value!)
            : query.ProductId;

        var normalizedQuery = new LotQuery
        {
            ProductId = selectedProductId,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            OrderBy = query.OrderBy,
            OrderDir = query.OrderDir,
            Code = query.Code,
            Status = query.Status,
            ExpirationFrom = query.ExpirationFrom,
            ExpirationTo = query.ExpirationTo,
            IsActive = query.IsActive,
            IncludeInactive = query.IncludeInactive
        };

        var result = await _lotsClient.ListAsync(normalizedQuery, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load lots.";
            return View(new LotListPageViewModel
            {
                Products = productOptions,
                Query = normalizedQuery
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Lots",
            Query = new Dictionary<string, string?>
            {
                ["ProductId"] = normalizedQuery.ProductId.ToString(),
                ["OrderBy"] = normalizedQuery.OrderBy,
                ["OrderDir"] = normalizedQuery.OrderDir,
                ["Code"] = normalizedQuery.Code,
                ["Status"] = normalizedQuery.Status?.ToString(),
                ["ExpirationFrom"] = normalizedQuery.ExpirationFrom?.ToString("yyyy-MM-dd"),
                ["ExpirationTo"] = normalizedQuery.ExpirationTo?.ToString("yyyy-MM-dd"),
                ["IsActive"] = normalizedQuery.IsActive?.ToString(),
                ["IncludeInactive"] = normalizedQuery.IncludeInactive.ToString(),
                ["PageSize"] = normalizedQuery.PageSize.ToString()
            }
        };

        var model = new LotListPageViewModel
        {
            Items = result.Data.Items,
            Query = normalizedQuery,
            Pagination = pagination,
            Products = productOptions
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _lotsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Lot not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? productId, CancellationToken cancellationToken)
    {
        var products = await LoadProductOptionsAsync(productId ?? Guid.Empty, cancellationToken);
        if (products.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No product found",
                Message = "Lots depend on a product. Do you want to create a product now?",
                PrimaryActionText = "Create product",
                PrimaryActionUrl = Url.Action("Create", "Products") ?? "#",
                SecondaryActionText = "Back to lots",
                SecondaryActionUrl = Url.Action("Index", "Lots") ?? "#",
                IconClass = "bi bi-upc"
            };
            return View("DependencyPrompt", prompt);
        }

        var selectedProductId = productId.HasValue && products.Any(p => p.Value == productId.Value.ToString())
            ? productId.Value
            : Guid.Parse(products[0].Value!);

        var model = new LotFormViewModel
        {
            ProductId = selectedProductId
        };

        ViewBag.Products = products;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LotFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Products = await LoadProductOptionsAsync(model.ProductId, cancellationToken);
            return View(model);
        }

        var result = await _lotsClient.CreateAsync(model.ProductId, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create lot.");
            ViewBag.Products = await LoadProductOptionsAsync(model.ProductId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Lot created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _lotsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Lot not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new LotFormViewModel
        {
            ProductId = result.Data.ProductId,
            Code = result.Data.Code,
            ManufactureDate = result.Data.ManufactureDate,
            ExpirationDate = result.Data.ExpirationDate,
            Status = result.Data.Status
        };

        ViewBag.Products = await LoadProductOptionsAsync(model.ProductId, cancellationToken);
        ViewBag.LotId = id;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, LotFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Products = await LoadProductOptionsAsync(model.ProductId, cancellationToken);
            ViewBag.LotId = id;
            return View(model);
        }

        var result = await _lotsClient.UpdateAsync(id, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update lot.");
            ViewBag.Products = await LoadProductOptionsAsync(model.ProductId, cancellationToken);
            ViewBag.LotId = id;
            return View(model);
        }

        TempData["Success"] = "Lot updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _lotsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Lot not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _lotsClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate lot.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Lot deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadProductOptionsAsync(Guid selectedId, CancellationToken cancellationToken)
    {
        var result = await _productsClient.ListAsync(
            new ProductQuery(1, 100, "Name", "asc", null, null, null, null, null, null, false),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<SelectListItem>();
        }

        return result.Data.Items
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), item.Id == selectedId))
            .ToList();
    }
}
