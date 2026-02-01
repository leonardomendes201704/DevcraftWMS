using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Settings.Queries.GetApiSettings;

public sealed record GetApiSettingsQuery() : IRequest<RequestResult<ApiSettingsDto>>;

