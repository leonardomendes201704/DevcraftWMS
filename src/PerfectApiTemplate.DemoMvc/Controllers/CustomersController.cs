using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.DemoMvc.ApiClients;
using PerfectApiTemplate.DemoMvc.ViewModels.Customers;
using PerfectApiTemplate.DemoMvc.ViewModels.Shared;

namespace PerfectApiTemplate.DemoMvc.Controllers;

public sealed class CustomersController : Controller
{
    private readonly CustomersApiClient _client;

    public CustomersController(CustomersApiClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] CustomerListQuery query, CancellationToken cancellationToken)
    {
        var result = await _client.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load customers.";
            return View(new CustomerListViewModel());
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Customers",
            Query = new Dictionary<string, string?>
            {
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["Search"] = query.Search,
                ["Email"] = query.Email,
                ["Name"] = query.Name,
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        var model = new CustomerListViewModel
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
            TempData["Error"] = result.Error ?? "Customer not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new CustomerDetailsViewModel { Customer = result.Data });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CustomerFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new CustomerCreateRequest(model.Name, model.Email, model.DateOfBirth);
        var result = await _client.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create customer.");
            return View(model);
        }

        TempData["Success"] = "Customer created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Customer not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new CustomerFormViewModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name,
            Email = result.Data.Email,
            DateOfBirth = result.Data.DateOfBirth
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CustomerFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            return View(model);
        }

        var request = new CustomerUpdateRequest(model.Name, model.Email, model.DateOfBirth);
        var result = await _client.UpdateAsync(model.Id.Value, request, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update customer.");
            return View(model);
        }

        TempData["Success"] = "Customer updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Customer not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new CustomerDetailsViewModel { Customer = result.Data });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to delete customer.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Customer deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }
}

