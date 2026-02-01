using DevcraftWMS.Application.Features.Settings;

namespace DevcraftWMS.Application.Abstractions.Settings;

public interface IAppSettingsReader
{
    Task<ApiSettingsDto> GetAsync(CancellationToken cancellationToken);
}

