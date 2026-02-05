using System.Diagnostics;
using System.Text;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.Sqlite;
using Microsoft.OpenApi;
using Microsoft.Extensions.Options;
using DevcraftWMS.Api.Middleware;
using DevcraftWMS.Api.Services;
using DevcraftWMS.Application;
using DevcraftWMS.Application.Abstractions.Logging;
using DevcraftWMS.Application.Abstractions.Settings;
using DevcraftWMS.Application.Abstractions.Telemetry;
using DevcraftWMS.Application.Abstractions.Storage;
using DevcraftWMS.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
EnsureSqliteDataDirectories(builder);

// --------------------------
// Logging (Serilog)
// --------------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// --------------------------
// MVC
// --------------------------
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DemoCors", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        if (origins.Length == 0)
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy
                .WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DevcraftWMS.Application.Abstractions.Auth.ICurrentUserService, CurrentUserService>();
builder.Services.Configure<LoggingOptions>(builder.Configuration.GetSection("Logging"));
builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection("Telemetry"));
builder.Services.AddOptions<CustomerContextOptions>()
    .Bind(builder.Configuration.GetSection("CustomerContext"))
    .Validate(options => !string.IsNullOrWhiteSpace(options.HeaderName), "CustomerContext:HeaderName is required.")
    .ValidateOnStart();
builder.Services.AddOptions<DashboardOptions>()
    .Bind(builder.Configuration.GetSection("Dashboard"))
    .Validate(options => options.ExpiringLotsDays > 0, "Dashboard:ExpiringLotsDays must be greater than zero.")
    .Validate(options => options.InboundWindowDays > 0, "Dashboard:InboundWindowDays must be greater than zero.")
    .Validate(options => options.OutboundWindowDays > 0, "Dashboard:OutboundWindowDays must be greater than zero.")
    .ValidateOnStart();
builder.Services.AddOptions<DevcraftWMS.Application.Features.ReceiptDivergences.ReceiptDivergenceOptions>()
    .Bind(builder.Configuration.GetSection(DevcraftWMS.Application.Features.ReceiptDivergences.ReceiptDivergenceOptions.SectionName))
    .Validate(options => options.MaxEvidenceBytes > 0, "ReceiptDivergences:MaxEvidenceBytes must be greater than zero.")
    .ValidateOnStart();
builder.Services.AddOptions<DevcraftWMS.Application.Features.Receipts.ReceiptMeasurementOptions>()
    .Bind(builder.Configuration.GetSection(DevcraftWMS.Application.Features.Receipts.ReceiptMeasurementOptions.SectionName))
    .ValidateOnStart();
builder.Services.AddOptions<DevcraftWMS.Application.Features.InboundOrderNotifications.InboundOrderNotificationOptions>()
    .Bind(builder.Configuration.GetSection("Notifications:InboundOrders"))
    .Validate(options => !options.WebhookEnabled || !string.IsNullOrWhiteSpace(options.WebhookUrl),
        "Notifications:InboundOrders:WebhookUrl is required when WebhookEnabled is true.")
    .Validate(options => !options.WebhookEnabled || Uri.IsWellFormedUriString(options.WebhookUrl, UriKind.Absolute),
        "Notifications:InboundOrders:WebhookUrl must be an absolute URI when WebhookEnabled is true.")
    .Validate(options => options.WebhookTimeoutSeconds > 0, "Notifications:InboundOrders:WebhookTimeoutSeconds must be greater than zero.")
    .ValidateOnStart();
builder.Services.AddOptions<DevcraftWMS.Application.Features.OutboundOrderNotifications.OutboundOrderNotificationOptions>()
    .Bind(builder.Configuration.GetSection("Notifications:OutboundOrders"))
    .Validate(options => !options.WebhookEnabled || !string.IsNullOrWhiteSpace(options.WebhookUrl),
        "Notifications:OutboundOrders:WebhookUrl is required when WebhookEnabled is true.")
    .Validate(options => !options.WebhookEnabled || Uri.IsWellFormedUriString(options.WebhookUrl, UriKind.Absolute),
        "Notifications:OutboundOrders:WebhookUrl must be an absolute URI when WebhookEnabled is true.")
    .Validate(options => options.WebhookTimeoutSeconds > 0, "Notifications:OutboundOrders:WebhookTimeoutSeconds must be greater than zero.")
    .ValidateOnStart();
builder.Services.AddOptions<FileStorageOptions>()
    .Bind(builder.Configuration.GetSection(FileStorageOptions.SectionName))
    .Validate(options => !string.IsNullOrWhiteSpace(options.Provider), "FileStorage:Provider is required.")
    .Validate(options => !string.Equals(options.Provider, "FileSystem", StringComparison.OrdinalIgnoreCase) ||
                         !string.IsNullOrWhiteSpace(options.BasePath), "FileStorage:BasePath is required for FileSystem provider.")
    .Validate(options => string.IsNullOrWhiteSpace(options.BaseUrl) ||
                         Uri.IsWellFormedUriString(options.BaseUrl, UriKind.Absolute), "FileStorage:BaseUrl must be an absolute URI when set.")
    .Validate(options => options.MaxFileSizeBytes > 0, "FileStorage:MaxFileSizeBytes must be greater than zero.")
    .ValidateOnStart();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<LoggingOptions>>().Value);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<TelemetryOptions>>().Value);
builder.Services.AddSingleton<IAppSettingsReader, ApiSettingsReader>();
builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();
builder.Services.AddScoped<DevcraftWMS.Application.Abstractions.Customers.ICustomerContext, CustomerContext>();
builder.Services.AddSingleton<IValidateOptions<DevcraftWMS.Application.Features.Receipts.ReceiptMeasurementOptions>, DevcraftWMS.Application.Features.Receipts.ReceiptMeasurementOptionsValidation>();

// --------------------------
// DI (Application + Infrastructure)
// --------------------------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// --------------------------
// JWT Auth (dev-friendly defaults; fail-fast outside dev)
// --------------------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];
var jwtSigningKey = jwtSection["SigningKey"];

if (builder.Environment.IsDevelopment())
{
    jwtIssuer ??= "dev-issuer";
    jwtAudience ??= "dev-audience";

    if (string.IsNullOrWhiteSpace(jwtSigningKey))
    {
        jwtSigningKey = "dev-signing-key-change-me";
        Log.Warning("Jwt:SigningKey was not set. Using a development default signing key. DO NOT use this in production.");
    }
}
else
{
    if (string.IsNullOrWhiteSpace(jwtIssuer) ||
        string.IsNullOrWhiteSpace(jwtAudience) ||
        string.IsNullOrWhiteSpace(jwtSigningKey))
    {
        throw new InvalidOperationException(
            "JWT configuration is missing. Please set Jwt:Issuer, Jwt:Audience, and Jwt:SigningKey for non-Development environments.");
    }
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey!)),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Role:Admin", policy =>
        policy.RequireRole(DevcraftWMS.Domain.Enums.UserRole.Admin.ToString()));
    options.AddPolicy("Role:Backoffice", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Admin.ToString()) ||
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Backoffice.ToString())));
    options.AddPolicy("Role:Portaria", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Admin.ToString()) ||
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Portaria.ToString())));
    options.AddPolicy("Role:Conferente", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Admin.ToString()) ||
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Conferente.ToString())));
    options.AddPolicy("Role:Qualidade", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Admin.ToString()) ||
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Qualidade.ToString())));
    options.AddPolicy("Role:Putaway", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Admin.ToString()) ||
            context.User.IsInRole(DevcraftWMS.Domain.Enums.UserRole.Putaway.ToString())));
});

// --------------------------
// Health Checks
// --------------------------
builder.Services.AddHealthChecks();

// --------------------------
// ProblemDetails + CorrelationId
// --------------------------
builder.Services.AddProblemDetails(options =>
{
    var requestIdHeader = builder.Configuration.GetValue<string>("Logging:RequestIdHeader") ?? "X-Request-Id";
    options.IncludeExceptionDetails = (context, _) =>
        builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing");
    options.OnBeforeWriteDetails = (context, details) =>
    {
        if (context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value) && value is string correlationId)
        {
            details.Extensions["correlationId"] = correlationId;
        }

        if (context.Items.TryGetValue(requestIdHeader, out var requestValue) && requestValue is string requestId)
        {
            details.Extensions["requestId"] = requestId;
        }
    };
});

// --------------------------
// Swagger (JWT)
// Ensures UI is available at: /swagger  (index is /swagger/index.html)
// --------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DevcraftWMS", Version = "v1" });

    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", document, "Bearer"), new List<string>() }
    });
});

var app = builder.Build();

// --------------------------
// Middleware pipeline
// --------------------------
app.UseSerilogRequestLogging();

// CorrelationId must run BEFORE ProblemDetails so errors include correlationId
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseProblemDetails();
app.UseMiddleware<ErrorLoggingMiddleware>();

// Swagger JSON endpoint: /swagger/v1/swagger.json
app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

// Swagger UI endpoint: /swagger (index: /swagger/index.html)
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevcraftWMS v1");
});

// --------------------------
// Auto-open browser on startup (Development only)
// Opens: /swagger/index.html
// --------------------------
if (app.Environment.IsDevelopment() && app.Configuration.GetValue("Swagger:OpenOnStart", true))
{
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
        {
            var server = app.Services.GetRequiredService<IServer>();
            var addressesFeature = server.Features.Get<IServerAddressesFeature>();
            var baseAddress = addressesFeature?.Addresses?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                return;
            }

            // Replace wildcard host with localhost for browser
            baseAddress = baseAddress
                .Replace("0.0.0.0", "localhost")
                .Replace("[::]", "localhost")
                .TrimEnd('/');

            var swaggerUrl = $"{baseAddress}/swagger/index.html";

            Process.Start(new ProcessStartInfo
            {
                FileName = swaggerUrl,
                UseShellExecute = true
            });
        }
        catch
        {
            // Non-fatal: ignore if the environment cannot launch a browser.
        }
    });
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("DemoCors");
app.UseMiddleware<CustomerContextMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<DevcraftWMS.Infrastructure.Realtime.NotificationsHub>("/hubs/notifications")
    .RequireCors("DemoCors");

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/api/test/throw", (HttpContext _) => throw new InvalidOperationException("Test exception"));
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Infrastructure.Persistence.ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var logsDb = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Infrastructure.Persistence.Logging.LogsDbContext>();
    await logsDb.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Infrastructure.Auth.AdminUserSeeder>();
    await seeder.SeedAsync();

    var rbacSeeder = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Infrastructure.Auth.RbacSeeder>();
    await rbacSeeder.SeedAsync();

    var rbacUserSeeder = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Infrastructure.Auth.RbacUserSeeder>();
    await rbacUserSeeder.SeedAsync();

    var sampleSeeder = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Infrastructure.Seeding.SampleDataSeeder>();
    await sampleSeeder.SeedAsync();
}

app.Run();

static void EnsureSqliteDataDirectories(WebApplicationBuilder builder)
{
    NormalizeSqliteConnectionString(builder, "ConnectionStrings:MainDb");
    NormalizeSqliteConnectionString(builder, "ConnectionStrings:LogsDb");
}

static void NormalizeSqliteConnectionString(WebApplicationBuilder builder, string key)
{
    var connectionString = builder.Configuration[key];
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return;
    }

    var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
    if (string.IsNullOrWhiteSpace(sqliteBuilder.DataSource))
    {
        return;
    }

    var dataSource = sqliteBuilder.DataSource;
    if (!Path.IsPathRooted(dataSource))
    {
        dataSource = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, dataSource));
        sqliteBuilder.DataSource = dataSource;
    }

    var directory = Path.GetDirectoryName(dataSource);
    if (!string.IsNullOrWhiteSpace(directory))
    {
        Directory.CreateDirectory(directory);
    }

    builder.Configuration[key] = sqliteBuilder.ToString();
}

public partial class Program { }


