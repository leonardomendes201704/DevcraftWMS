namespace PerfectApiTemplate.DemoMvc.Infrastructure.Settings;

public interface IUiSettingsStore
{
    Task<UiSettingsData> LoadAsync(CancellationToken cancellationToken);
    Task SaveAsync(UiSettingsData settings, CancellationToken cancellationToken);
}
