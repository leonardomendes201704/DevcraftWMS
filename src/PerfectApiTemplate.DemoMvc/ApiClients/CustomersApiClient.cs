using PerfectApiTemplate.DemoMvc.ViewModels.Customers;

namespace PerfectApiTemplate.DemoMvc.ApiClients;

public sealed class CustomersApiClient : ApiClientBase
{
    public CustomersApiClient(
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

    public Task<ApiResult<PagedResultDto<CustomerDto>>> ListAsync(CustomerListQuery query, CancellationToken cancellationToken)
    {
        var url = $"/api/customers/paged?pageNumber={query.PageNumber}&pageSize={query.PageSize}&orderBy={query.OrderBy}&orderDir={query.OrderDir}" +
                  $"&search={Uri.EscapeDataString(query.Search ?? string.Empty)}&email={Uri.EscapeDataString(query.Email ?? string.Empty)}" +
                  $"&name={Uri.EscapeDataString(query.Name ?? string.Empty)}&includeInactive={query.IncludeInactive}";
        return GetAsync<PagedResultDto<CustomerDto>>(url, cancellationToken);
    }

    public Task<ApiResult<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<CustomerDto>($"/api/customers/{id}", cancellationToken);

    public Task<ApiResult<CustomerDto>> CreateAsync(CustomerCreateRequest request, CancellationToken cancellationToken)
        => PostAsync<CustomerDto>("/api/customers", request, cancellationToken);

    public Task<ApiResult<CustomerDto>> UpdateAsync(Guid id, CustomerUpdateRequest request, CancellationToken cancellationToken)
        => PutAsync<CustomerDto>($"/api/customers/{id}", request, cancellationToken);

    public Task<ApiResult<CustomerDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<CustomerDto>($"/api/customers/{id}", cancellationToken);
}

public sealed record CustomerDto(Guid Id, string Name, string Email, DateOnly DateOfBirth, DateTime CreatedAtUtc);

public sealed record CustomerCreateRequest(string Name, string Email, DateOnly DateOfBirth);

public sealed record CustomerUpdateRequest(string Name, string Email, DateOnly DateOfBirth);

public sealed record PagedResultDto<T>(IReadOnlyList<T> Items, int TotalCount, int PageNumber, int PageSize, string OrderBy, string OrderDir);
