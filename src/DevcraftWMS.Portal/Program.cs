using DevcraftWMS.Portal.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddOptions<PortalOptions>()
    .Bind(builder.Configuration)
    .Validate(options => Uri.TryCreate(options.ApiBaseUrl, UriKind.Absolute, out _), "Portal ApiBaseUrl must be an absolute URL.")
    .ValidateOnStart();
builder.Services.AddSingleton<ApiUrlProvider>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<DevcraftWMS.Portal.ApiClients.HealthApiClient>();
builder.Services.AddScoped<DevcraftWMS.Portal.ApiClients.AuthApiClient>();
builder.Services.AddScoped<DevcraftWMS.Portal.ApiClients.CustomersApiClient>();
builder.Services.AddScoped<DevcraftWMS.Portal.ApiClients.AsnsApiClient>();
builder.Services.AddScoped<DevcraftWMS.Portal.ApiClients.InboundOrdersApiClient>();
builder.Services.AddScoped<DevcraftWMS.Portal.ApiClients.WarehousesApiClient>();
builder.Services.AddScoped<DevcraftWMS.Portal.ApiClients.ProductsApiClient>();
builder.Services.AddScoped<DevcraftWMS.Portal.ApiClients.UomsApiClient>();

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
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

    var token = context.Session.GetStringValue(SessionKeys.JwtToken);
    if (string.IsNullOrWhiteSpace(token))
    {
        context.Response.Redirect("/Auth/Login");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
