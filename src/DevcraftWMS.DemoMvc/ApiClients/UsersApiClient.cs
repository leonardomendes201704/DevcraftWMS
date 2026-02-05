using DevcraftWMS.DemoMvc.ViewModels.Users;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class UsersApiClient : ApiClientBase
{
    public UsersApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider,
        Infrastructure.Telemetry.IClientCorrelationContext correlationContext,
        Infrastructure.Telemetry.IClientTelemetryDispatcher telemetryDispatcher,
        Microsoft.Extensions.Options.IOptionsMonitor<Infrastructure.Telemetry.ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
        : base(httpClient, httpContextAccessor, urlProvider, correlationContext, telemetryDispatcher, telemetryOptions, environment)
    {
    }

    public Task<ApiResult<PagedResultDto<UserListItemViewModel>>> ListAsync(UserQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/users";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["search"] = query.Search,
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<UserListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<UserDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<UserDetailViewModel>($"/api/users/{id}", cancellationToken);

    public Task<ApiResult<UserDetailViewModel>> CreateAsync(UserCreateViewModel payload, CancellationToken cancellationToken)
        => PostAsync<UserDetailViewModel>("/api/users", new CreateUserRequest(payload.Email, payload.FullName, payload.Password, payload.RoleIds), cancellationToken);

    public Task<ApiResult<UserDetailViewModel>> UpdateAsync(Guid id, UserEditViewModel payload, CancellationToken cancellationToken)
        => PutAsync<UserDetailViewModel>($"/api/users/{id}", new UpdateUserRequest(payload.Email, payload.FullName, payload.RoleIds), cancellationToken);

    public Task<ApiResult<UserDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<UserDetailViewModel>($"/api/users/{id}", cancellationToken);

    public Task<ApiResult<string>> ResetPasswordAsync(Guid id, CancellationToken cancellationToken)
        => PostAsync($"/api/users/{id}/reset-password", new { }, cancellationToken);

    public Task<ApiResult<string>> AssignRolesAsync(Guid id, IReadOnlyList<Guid> roleIds, CancellationToken cancellationToken)
        => PutAsync<string>($"/api/users/{id}/roles", new AssignUserRolesRequest(roleIds), cancellationToken);
}

public sealed record CreateUserRequest(string Email, string FullName, string Password, IReadOnlyList<Guid> RoleIds);

public sealed record UpdateUserRequest(string Email, string FullName, IReadOnlyList<Guid> RoleIds);

public sealed record AssignUserRolesRequest(IReadOnlyList<Guid> RoleIds);
