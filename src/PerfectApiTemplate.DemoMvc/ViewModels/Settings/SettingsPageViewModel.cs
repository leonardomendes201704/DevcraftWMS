using PerfectApiTemplate.DemoMvc.ApiClients;

namespace PerfectApiTemplate.DemoMvc.ViewModels.Settings;

public sealed class SettingsPageViewModel
{
    public SettingsViewModel Form { get; init; } = new();
    public ApiSettingsDto? ApiSettings { get; init; }
    public string? ApiSettingsError { get; init; }
    public IReadOnlyList<SettingsSectionViewModel> FrontendSections { get; init; } = Array.Empty<SettingsSectionViewModel>();
}

public sealed class SettingsSectionViewModel
{
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<SettingsItemViewModel> Items { get; init; } = Array.Empty<SettingsItemViewModel>();
}

public sealed class SettingsItemViewModel
{
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
