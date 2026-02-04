using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("App_Data/ui-settings.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientTelemetryExceptionFilter>();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddOptions<DevcraftWMS.DemoMvc.Infrastructure.DemoOptions>()
    .Bind(builder.Configuration)
    .Validate(options => Uri.TryCreate(options.ApiBaseUrl, UriKind.Absolute, out _), "DemoMvc ApiBaseUrl must be an absolute URL.")
    .ValidateOnStart();
builder.Services.AddSingleton<DevcraftWMS.DemoMvc.Infrastructure.ApiUrlProvider>();
builder.Services.Configure<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientTelemetryOptions>(builder.Configuration.GetSection("Telemetry"));
builder.Services.AddOptions<ExternalServicesOptions>()
    .Bind(builder.Configuration.GetSection("ExternalServices"))
    .ValidateOnStart();
builder.Services.AddSingleton<Microsoft.Extensions.Options.IValidateOptions<ExternalServicesOptions>, ExternalServicesOptionsValidation>();
builder.Services.AddSingleton<DevcraftWMS.DemoMvc.Infrastructure.Settings.IUiSettingsStore, DevcraftWMS.DemoMvc.Infrastructure.Settings.FileUiSettingsStore>();
builder.Services.AddSingleton<DevcraftWMS.DemoMvc.Infrastructure.Settings.FrontendSettingsReader>();
builder.Services.AddSingleton<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientTelemetryQueue>(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientTelemetryOptions>>().Value;
    var logger = sp.GetRequiredService<ILogger<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientTelemetryQueue>>();
    return new DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientTelemetryQueue(options.QueueCapacity, logger);
});
builder.Services.AddSingleton<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.IClientTelemetryDispatcher, DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientTelemetryDispatcher>();
builder.Services.AddSingleton<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.IClientCorrelationContext, DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientCorrelationContext>();
builder.Services.AddHostedService<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.TelemetryWorker>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.TelemetryApiClient>();
builder.Services.AddHttpClient<IIbgeClient, IbgeClient>((sp, client) =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExternalServicesOptions>>().Value;
    client.BaseAddress = new Uri(options.Ibge.BaseUrl);
});
builder.Services.AddHttpClient<IViaCepClient, ViaCepClient>((sp, client) =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExternalServicesOptions>>().Value;
    client.BaseAddress = new Uri(options.ViaCep.BaseUrl);
});
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.HealthApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.AuthApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.CustomersApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.DashboardApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.EmailApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.LogsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.SettingsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.WarehousesApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.SectorsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.SectionsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.StructuresApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.LocationsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.AislesApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.ZonesApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.ProductsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.LotsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.UomsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.InventoryBalancesApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.InventoryMovementsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.ReceiptsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.UnitLoadsApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.PutawayTasksApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.InboundOrdersApiClient>();
builder.Services.AddScoped<DevcraftWMS.DemoMvc.ApiClients.QualityInspectionsApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/Home/Error");
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientCorrelationMiddleware>();
app.UseSession();

app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    if (path.StartsWithSegments("/Auth", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/css", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/js", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/lib", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/favicon.ico", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }

    var token = context.Session.GetStringValue(DevcraftWMS.DemoMvc.Infrastructure.SessionKeys.JwtToken);
    if (string.IsNullOrWhiteSpace(token))
    {
        context.Response.Redirect("/Auth/Login");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

