using MediatR;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Logs.Queries.GetTransactionLogById;

public sealed class GetTransactionLogByIdQueryHandler : IRequestHandler<GetTransactionLogByIdQuery, RequestResult<TransactionLogDetailDto>>
{
    private readonly ITransactionLogReadRepository _repository;

    public GetTransactionLogByIdQueryHandler(ITransactionLogReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<TransactionLogDetailDto>> Handle(GetTransactionLogByIdQuery request, CancellationToken cancellationToken)
    {
        var log = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (log is null)
        {
            return RequestResult<TransactionLogDetailDto>.Failure("logs.transaction.not_found", "Transaction log not found.");
        }

        return RequestResult<TransactionLogDetailDto>.Success(log);
    }
}

