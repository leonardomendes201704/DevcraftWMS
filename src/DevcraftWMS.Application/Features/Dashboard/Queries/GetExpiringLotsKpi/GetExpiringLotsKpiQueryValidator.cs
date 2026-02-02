using FluentValidation;

namespace DevcraftWMS.Application.Features.Dashboard.Queries.GetExpiringLotsKpi;

public sealed class GetExpiringLotsKpiQueryValidator : AbstractValidator<GetExpiringLotsKpiQuery>
{
    public GetExpiringLotsKpiQueryValidator()
    {
        RuleFor(x => x.WindowDays).InclusiveBetween(1, 3650);
    }
}
