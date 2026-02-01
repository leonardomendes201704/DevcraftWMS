using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Logs.Queries.GetTransactionLogById;

public sealed record GetTransactionLogByIdQuery(Guid Id) : IRequest<RequestResult<TransactionLogDetailDto>>;


