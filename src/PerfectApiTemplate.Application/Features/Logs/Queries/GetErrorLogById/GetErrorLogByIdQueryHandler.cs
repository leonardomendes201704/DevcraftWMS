using MediatR;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Logs.Queries.GetErrorLogById;

public sealed class GetErrorLogByIdQueryHandler : IRequestHandler<GetErrorLogByIdQuery, RequestResult<ErrorLogDetailDto>>
{
    private readonly IErrorLogReadRepository _repository;

    public GetErrorLogByIdQueryHandler(IErrorLogReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<ErrorLogDetailDto>> Handle(GetErrorLogByIdQuery request, CancellationToken cancellationToken)
    {
        var log = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (log is null)
        {
            return RequestResult<ErrorLogDetailDto>.Failure("logs.error.not_found", "Error log not found.");
        }

        return RequestResult<ErrorLogDetailDto>.Success(log);
    }
}

