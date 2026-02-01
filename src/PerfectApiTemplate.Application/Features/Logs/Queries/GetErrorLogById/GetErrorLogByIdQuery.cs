using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Logs.Queries.GetErrorLogById;

public sealed record GetErrorLogByIdQuery(Guid Id) : IRequest<RequestResult<ErrorLogDetailDto>>;

