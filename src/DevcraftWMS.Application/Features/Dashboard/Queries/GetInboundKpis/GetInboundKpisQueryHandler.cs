using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Dashboard.Queries.GetInboundKpis;

public sealed class GetInboundKpisQueryHandler : IRequestHandler<GetInboundKpisQuery, RequestResult<InboundKpiDto>>
{
    private readonly IDashboardKpiRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetInboundKpisQueryHandler(IDashboardKpiRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<InboundKpiDto>> Handle(GetInboundKpisQuery request, CancellationToken cancellationToken)
    {
        var endUtc = _dateTimeProvider.UtcNow;
        var startUtc = endUtc.AddDays(-request.WindowDays);

        var counts = await _repository.GetInboundKpisAsync(startUtc, endUtc, cancellationToken);

        var dto = new InboundKpiDto(
            request.WindowDays,
            startUtc,
            endUtc,
            counts.Arrivals,
            counts.DockAssigned,
            counts.ReceiptsCompleted,
            counts.PutawayCompleted);

        return RequestResult<InboundKpiDto>.Success(dto);
    }
}
