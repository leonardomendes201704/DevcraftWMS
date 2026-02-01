using System.Text.Json;

namespace PerfectApiTemplate.DemoMvc.Infrastructure.Settings;

public sealed class FileUiSettingsStore : IUiSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly IWebHostEnvironment _environment;

    public FileUiSettingsStore(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<UiSettingsData> LoadAsync(CancellationToken cancellationToken)
    {
        var path = GetSettingsPath();
        if (!File.Exists(path))
        {
            return new UiSettingsData();
        }

        await using var stream = File.OpenRead(path);
        var settings = await JsonSerializer.DeserializeAsync<UiSettingsData>(stream, JsonOptions, cancellationToken);
        return settings ?? new UiSettingsData();
    }

    public async Task SaveAsync(UiSettingsData settings, CancellationToken cancellationToken)
    {
        var path = GetSettingsPath();
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, settings, JsonOptions, cancellationToken);
    }

    private string GetSettingsPath()
    {
        return Path.Combine(_environment.ContentRootPath, "App_Data", "ui-settings.json");
    }
}
