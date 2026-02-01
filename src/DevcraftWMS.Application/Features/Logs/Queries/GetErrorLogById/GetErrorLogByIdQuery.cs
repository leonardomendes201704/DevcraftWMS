using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Logs.Queries.GetErrorLogById;

public sealed record GetErrorLogByIdQuery(Guid Id) : IRequest<RequestResult<ErrorLogDetailDto>>;


