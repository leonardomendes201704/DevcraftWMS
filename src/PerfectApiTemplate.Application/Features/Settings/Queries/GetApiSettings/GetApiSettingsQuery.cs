using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Settings.Queries.GetApiSettings;

public sealed record GetApiSettingsQuery() : IRequest<RequestResult<ApiSettingsDto>>;
