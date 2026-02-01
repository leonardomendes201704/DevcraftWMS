using MediatR;
using DevcraftWMS.Application.Abstractions.Settings;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Settings.Queries.GetApiSettings;

public sealed class GetApiSettingsQueryHandler : IRequestHandler<GetApiSettingsQuery, RequestResult<ApiSettingsDto>>
{
    private readonly IAppSettingsReader _reader;

    public GetApiSettingsQueryHandler(IAppSettingsReader reader)
    {
        _reader = reader;
    }

    public async Task<RequestResult<ApiSettingsDto>> Handle(GetApiSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _reader.GetAsync(cancellationToken);
        return RequestResult<ApiSettingsDto>.Success(settings);
    }
}

