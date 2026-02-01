using PerfectApiTemplate.DemoMvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("App_Data/ui-settings.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientTelemetryExceptionFilter>();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<PerfectApiTemplate.DemoMvc.Infrastructure.DemoOptions>(builder.Configuration);
builder.Services.AddSingleton<PerfectApiTemplate.DemoMvc.Infrastructure.ApiUrlProvider>();
builder.Services.Configure<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientTelemetryOptions>(builder.Configuration.GetSection("Telemetry"));
builder.Services.AddSingleton<PerfectApiTemplate.DemoMvc.Infrastructure.Settings.IUiSettingsStore, PerfectApiTemplate.DemoMvc.Infrastructure.Settings.FileUiSettingsStore>();
builder.Services.AddSingleton<PerfectApiTemplate.DemoMvc.Infrastructure.Settings.FrontendSettingsReader>();
builder.Services.AddSingleton<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientTelemetryQueue>(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientTelemetryOptions>>().Value;
    var logger = sp.GetRequiredService<ILogger<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientTelemetryQueue>>();
    return new PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientTelemetryQueue(options.QueueCapacity, logger);
});
builder.Services.AddSingleton<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.IClientTelemetryDispatcher, PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientTelemetryDispatcher>();
builder.Services.AddSingleton<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.IClientCorrelationContext, PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientCorrelationContext>();
builder.Services.AddHostedService<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.TelemetryWorker>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.TelemetryApiClient>();
builder.Services.AddScoped<PerfectApiTemplate.DemoMvc.ApiClients.HealthApiClient>();
builder.Services.AddScoped<PerfectApiTemplate.DemoMvc.ApiClients.AuthApiClient>();
builder.Services.AddScoped<PerfectApiTemplate.DemoMvc.ApiClients.CustomersApiClient>();
builder.Services.AddScoped<PerfectApiTemplate.DemoMvc.ApiClients.EmailApiClient>();
builder.Services.AddScoped<PerfectApiTemplate.DemoMvc.ApiClients.LogsApiClient>();
builder.Services.AddScoped<PerfectApiTemplate.DemoMvc.ApiClients.SettingsApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/Home/Error");
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientCorrelationMiddleware>();
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

    var token = context.Session.GetStringValue(PerfectApiTemplate.DemoMvc.Infrastructure.SessionKeys.JwtToken);
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
