using System.Diagnostics;
using System.Text;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Api.Middleware;
using PerfectApiTemplate.Api.Services;
using PerfectApiTemplate.Application;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Abstractions.Settings;
using PerfectApiTemplate.Application.Abstractions.Telemetry;
using PerfectApiTemplate.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<PerfectApiTemplate.Application.Abstractions.Auth.ICurrentUserService, CurrentUserService>();
builder.Services.Configure<LoggingOptions>(builder.Configuration.GetSection("Logging"));
builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection("Telemetry"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<LoggingOptions>>().Value);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<TelemetryOptions>>().Value);
builder.Services.AddSingleton<IAppSettingsReader, ApiSettingsReader>();
builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey!))
        };
    });

builder.Services.AddAuthorization();

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
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Perfect API Template", Version = "v1" });

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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Perfect API Template v1");
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
            // Resolve actual bound addresses (e.g., http://localhost:5000)
            var server = app.Services.GetRequiredService<IServer>();
            var addressesFeature = server.Features.Get<IServerAddressesFeature>();
            var baseAddress = addressesFeature?.Addresses?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                // Fallback if addresses not available
                baseAddress = "http://localhost:5000";
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

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<PerfectApiTemplate.Infrastructure.Realtime.NotificationsHub>("/hubs/notifications")
    .RequireCors("DemoCors");

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/api/test/throw", (HttpContext _) => throw new InvalidOperationException("Test exception"));
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PerfectApiTemplate.Infrastructure.Persistence.ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var logsDb = scope.ServiceProvider.GetRequiredService<PerfectApiTemplate.Infrastructure.Persistence.Logging.LogsDbContext>();
    await logsDb.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<PerfectApiTemplate.Infrastructure.Auth.AdminUserSeeder>();
    await seeder.SeedAsync();
}

app.Run();

public partial class Program { }

