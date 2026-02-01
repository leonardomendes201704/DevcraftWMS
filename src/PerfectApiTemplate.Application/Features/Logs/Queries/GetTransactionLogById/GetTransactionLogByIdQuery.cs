using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Logs.Queries.GetTransactionLogById;

public sealed record GetTransactionLogByIdQuery(Guid Id) : IRequest<RequestResult<TransactionLogDetailDto>>;

