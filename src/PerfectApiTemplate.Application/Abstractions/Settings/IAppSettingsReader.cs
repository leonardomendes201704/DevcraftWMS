using PerfectApiTemplate.Application.Features.Settings;

namespace PerfectApiTemplate.Application.Abstractions.Settings;

public interface IAppSettingsReader
{
    Task<ApiSettingsDto> GetAsync(CancellationToken cancellationToken);
}
