namespace PerfectApiTemplate.Application.Features.Settings;

public sealed record ApiSettingsDto(
    IReadOnlyList<ApiSettingsSectionDto> Sections,
    DateTimeOffset RetrievedAtUtc);

public sealed record ApiSettingsSectionDto(
    string Name,
    IReadOnlyList<ApiSettingItemDto> Items);

public sealed record ApiSettingItemDto(
    string Key,
    string Value,
    string Description);
