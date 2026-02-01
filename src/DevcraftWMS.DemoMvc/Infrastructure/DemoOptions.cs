namespace DevcraftWMS.DemoMvc.Infrastructure;

public sealed class DemoOptions
{
    public string ApiBaseUrl { get; set; } = string.Empty;
    public DemoDefaults Demo { get; set; } = new();
}

public sealed class DemoDefaults
{
    public string AdminEmail { get; set; } = string.Empty;
}


