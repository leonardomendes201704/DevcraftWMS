using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Roles;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.Users;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class UsersController : Controller
{
    private readonly UsersApiClient _usersClient;
    private readonly RolesApiClient _rolesClient;

    public UsersController(UsersApiClient usersClient, RolesApiClient rolesClient)
    {
        _usersClient = usersClient;
        _rolesClient = rolesClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] UserQuery query, CancellationToken cancellationToken)
    {
        var result = await _usersClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load users.";
            return View(new UserListPageViewModel());
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Users",
            Query = new Dictionary<string, string?>
            {
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["Search"] = query.Search,
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        var model = new UserListPageViewModel
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
        var result = await _usersClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "User not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new UserDetailsPageViewModel { User = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new UserCreateViewModel
        {
            AvailableRoles = await LoadRolesAsync(Array.Empty<Guid>(), cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = await LoadRolesAsync(model.RoleIds, cancellationToken);
            return View(model);
        }

        var result = await _usersClient.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create user.");
            model.AvailableRoles = await LoadRolesAsync(model.RoleIds, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "User created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _usersClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "User not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new UserEditViewModel
        {
            Id = result.Data.Id,
            Email = result.Data.Email,
            FullName = result.Data.FullName,
            RoleIds = await LoadRoleIdsByNamesAsync(result.Data.Roles, cancellationToken)
        };

        model.AvailableRoles = await LoadRolesAsync(model.RoleIds, cancellationToken);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = await LoadRolesAsync(model.RoleIds, cancellationToken);
            return View(model);
        }

        var result = await _usersClient.UpdateAsync(model.Id, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update user.");
            model.AvailableRoles = await LoadRolesAsync(model.RoleIds, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "User updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _usersClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "User not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new UserDetailsPageViewModel { User = result.Data });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _usersClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate user.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "User deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(Guid id, CancellationToken cancellationToken)
    {
        var result = await _usersClient.ResetPasswordAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to reset password.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Password reset to default.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadRolesAsync(IReadOnlyCollection<Guid> selected, CancellationToken cancellationToken)
    {
        var query = new RoleQuery(1, 100, "Name", "asc", null, true, true);
        var result = await _rolesClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load roles.";
            return Array.Empty<SelectListItem>();
        }

        return result.Data.Items
            .OrderBy(r => r.Name)
            .Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name,
                Selected = selected.Contains(r.Id)
            })
            .ToList();
    }

    private async Task<IReadOnlyList<Guid>> LoadRoleIdsByNamesAsync(IReadOnlyCollection<string> roleNames, CancellationToken cancellationToken)
    {
        if (roleNames.Count == 0)
        {
            return Array.Empty<Guid>();
        }

        var query = new RoleQuery(1, 100, "Name", "asc", null, true, true);
        var result = await _rolesClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Array.Empty<Guid>();
        }

        return result.Data.Items
            .Where(r => roleNames.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
            .Select(r => r.Id)
            .ToList();
    }
}
