using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Dashboard.Queries.GetExpiringLotsKpi;

public sealed class GetExpiringLotsKpiQueryHandler : IRequestHandler<GetExpiringLotsKpiQuery, RequestResult<ExpiringLotsKpiDto>>
{
    private readonly ILotRepository _lotRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetExpiringLotsKpiQueryHandler(ILotRepository lotRepository, IDateTimeProvider dateTimeProvider)
    {
        _lotRepository = lotRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<ExpiringLotsKpiDto>> Handle(GetExpiringLotsKpiQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(_dateTimeProvider.UtcNow);
        var endDate = today.AddDays(request.WindowDays);

        var total = await _lotRepository.CountExpiringAsync(
            today,
            endDate,
            request.Status,
            cancellationToken);

        return RequestResult<ExpiringLotsKpiDto>.Success(new ExpiringLotsKpiDto(total, request.WindowDays));
    }
}
