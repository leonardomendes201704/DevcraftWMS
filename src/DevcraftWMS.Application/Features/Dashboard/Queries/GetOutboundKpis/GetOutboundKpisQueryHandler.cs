using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Dashboard.Queries.GetOutboundKpis;

public sealed class GetOutboundKpisQueryHandler : IRequestHandler<GetOutboundKpisQuery, RequestResult<OutboundKpiDto>>
{
    private readonly IDashboardKpiRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetOutboundKpisQueryHandler(IDashboardKpiRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<OutboundKpiDto>> Handle(GetOutboundKpisQuery request, CancellationToken cancellationToken)
    {
        var endUtc = _dateTimeProvider.UtcNow;
        var startUtc = endUtc.AddDays(-request.WindowDays);

        var counts = await _repository.GetOutboundKpisAsync(startUtc, endUtc, cancellationToken);
        var dto = new OutboundKpiDto(
            request.WindowDays,
            startUtc,
            endUtc,
            counts.PickingCompleted,
            counts.ChecksCompleted,
            counts.ShipmentsCompleted);

        return RequestResult<OutboundKpiDto>.Success(dto);
    }
}
