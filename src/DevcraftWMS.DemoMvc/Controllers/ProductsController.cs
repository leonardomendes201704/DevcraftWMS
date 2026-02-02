using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Products;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Uoms;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class ProductsController : Controller
{
    private readonly ProductsApiClient _productsClient;
    private readonly UomsApiClient _uomsClient;

    public ProductsController(ProductsApiClient productsClient, UomsApiClient uomsClient)
    {
        _productsClient = productsClient;
        _uomsClient = uomsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ProductQuery query, CancellationToken cancellationToken)
    {
        var result = await _productsClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load products.";
            return View(new ProductListPageViewModel { Query = query });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Products",
            Query = new Dictionary<string, string?>
            {
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["Code"] = query.Code,
                ["Name"] = query.Name,
                ["Category"] = query.Category,
                ["Brand"] = query.Brand,
                ["Ean"] = query.Ean,
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        var model = new ProductListPageViewModel
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
        var productResult = await _productsClient.GetByIdAsync(id, cancellationToken);
        if (!productResult.IsSuccess || productResult.Data is null)
        {
            TempData["Error"] = productResult.Error ?? "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        var uomsResult = await _productsClient.ListProductUomsAsync(id, cancellationToken);
        var uoms = uomsResult.IsSuccess && uomsResult.Data is not null
            ? uomsResult.Data
            : Array.Empty<ProductUomListItemViewModel>();

        var availableUoms = await LoadUomOptionsAsync(uoms, productResult.Data.BaseUomId, cancellationToken);

        var model = new ProductDetailsPageViewModel
        {
            Product = productResult.Data,
            Uoms = uoms,
            AvailableUoms = availableUoms,
            NewUom = new ProductUomCreateViewModel()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddUom(Guid id, ProductUomCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please provide a valid UoM and conversion factor.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _productsClient.AddProductUomAsync(id, model, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to add UoM conversion.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "UoM conversion added.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var baseUoms = await LoadAllUomOptionsAsync(null, cancellationToken);
        if (baseUoms.Count == 0)
        {
            var prompt = new DependencyPromptViewModel
            {
                Title = "No UoM found",
                Message = "Products require a base unit of measure. Do you want to create a UoM now?",
                PrimaryActionText = "Create UoM",
                PrimaryActionUrl = Url.Action("Create", "Uoms") ?? "#",
                SecondaryActionText = "Back to products",
                SecondaryActionUrl = Url.Action("Index", "Products") ?? "#",
                IconClass = "bi bi-rulers"
            };
            return View("DependencyPrompt", prompt);
        }

        var model = new ProductFormViewModel
        {
            BaseUoms = baseUoms
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.BaseUoms = await LoadAllUomOptionsAsync(model.BaseUomId, cancellationToken);
            return View(model);
        }

        var result = await _productsClient.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create product.");
            model.BaseUoms = await LoadAllUomOptionsAsync(model.BaseUomId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new ProductFormViewModel
        {
            Id = result.Data.Id,
            Code = result.Data.Code,
            Name = result.Data.Name,
            Description = result.Data.Description,
            Ean = result.Data.Ean,
            ErpCode = result.Data.ErpCode,
            Category = result.Data.Category,
            Brand = result.Data.Brand,
            BaseUomId = result.Data.BaseUomId,
            WeightKg = result.Data.WeightKg,
            LengthCm = result.Data.LengthCm,
            WidthCm = result.Data.WidthCm,
            HeightCm = result.Data.HeightCm,
            VolumeCm3 = result.Data.VolumeCm3,
            BaseUoms = await LoadAllUomOptionsAsync(result.Data.BaseUomId, cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            model.BaseUoms = await LoadAllUomOptionsAsync(model.BaseUomId, cancellationToken);
            return View(model);
        }

        var result = await _productsClient.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update product.");
            model.BaseUoms = await LoadAllUomOptionsAsync(model.BaseUomId, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productsClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate product.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Product deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadAllUomOptionsAsync(Guid? selectedId, CancellationToken cancellationToken)
    {
        var result = await _uomsClient.ListAsync(
            new UomQuery(
                1,
                200,
                "Code",
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
            .Select(item => new SelectListItem($"{item.Code} - {item.Name}", item.Id.ToString(), selectedId.HasValue && item.Id == selectedId.Value))
            .ToList();
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadUomOptionsAsync(
        IReadOnlyList<ProductUomListItemViewModel> assigned,
        Guid baseUomId,
        CancellationToken cancellationToken)
    {
        var assignedUoms = new HashSet<Guid>(assigned.Select(x => x.UomId)) { baseUomId };
        var all = await LoadAllUomOptionsAsync(null, cancellationToken);

        return all.Where(uom => Guid.TryParse(uom.Value, out var id) && !assignedUoms.Contains(id)).ToList();
    }
}
