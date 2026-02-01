namespace PerfectApiTemplate.DemoMvc.Infrastructure;

public sealed class DemoOptions
{
    public string ApiBaseUrl { get; set; } = "https://localhost:5001";
    public DemoDefaults Demo { get; set; } = new();
}

public sealed class DemoDefaults
{
    public string AdminEmail { get; set; } = "admin@admin.com.br";
}

